using FarmTypeManager.CustomActions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using xTile.Tiles;

namespace FarmTypeManager.Serialization
{
    /// <summary>The serializer used to save and load <see cref="Monster"/> instances.</summary>
    /// <remarks>
    /// <para>
    /// This mod uses workaround serializers to save and load custom types that aren't recognized by the game's save seralizer.
    /// The game will throw errors and freeze if it attempts to save unrecognized in-game types.
    /// These serializers remove instances before the game saves, save them elsewhere, and re-add them after the main save process.
    /// </para>
    /// <para>
    /// As of this writing, this serialization method cannot account for instances that move between different <see cref="GameLocation"/>s.
    /// </para>
    /// </remarks>
    public static class MonsterSerializer
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The key to use for this class's save data.</summary>
        /// <remarks>This is intended for use with methods like <see cref="IDataHelper.WriteJsonFile"/>, which don't require keys to be unique between mods.</remarks>
        public static string SaveDataKey { get; } = "MonsterSerializer";

        /// <summary>A set of all instances' serializer IDs and serialization data.</summary>
        private static Dictionary<string, MonsterData> IDsAndData { get; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>A set of names locations with tracked instances. Used to improve performance when saving, removing, etc.</summary>
        /// <remarks>Each key is location's <see cref="GameLocation.NameOrUniqueName"/>. Case-insensitive.</remarks>
        private static HashSet<string> LocationNames { get; } = new(StringComparer.OrdinalIgnoreCase);

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Initializes this serializer's events and other setup tasks.</summary>
        /// <remarks>
        /// <para>To describe the intended flow of these events:</para>
        /// <list type="number">
        /// <item>
        /// <para>Before a session loads, this class should clear its tracked instances.</para>
        /// <para>This prevents tracking instances from a previously loaded session. It may not be strictly necessary, and it requires correct timing, but improves performance when saving.</para>
        /// </item>
        /// <item>
        /// <para>After a session loads, this class should run its "load and add to world" method.</para>
        /// <para>That method places any instances from save data into the game, and then clears this class's save data. (Clearing the save data doesn't become permanent until the game actually saves.)</para>
        /// </item>
        /// <item>
        /// <para>Before a game session saves, this class should run its "save and remove from world" method.</para>
        /// <para>That method finds any tracked instances that still exist in-game, removes them, creates save data for them, clears all tracked instances, then updates the save data.</para>
        /// </item>
        /// <item>
        /// <para>After a game session saves, this class should run its "load and add to world" method. (See "2.")</para>
        /// </item>
        /// </list>
        /// <para>
        /// It's possible that these events will be called multiple times in succession, e.g. due to both normal SMAPI events and quick-save events.
        /// The "save" and "load" methods should be careful not to overwrite their own work in these cases.
        /// For example, the "save" method should do nothing if it detects that unloaded save data exist (e.g. it's not null).
        /// The "load" event should null save data after loading, to allow future saves while preventing itself from loading multiple sets.
        /// </para>
        /// </remarks>
        public static void Initialize()
        {
            //serialization

            FTMUtility.Helper.Events.GameLoop.ReturnedToTitle += (_, _) => Clear(); //NOTE: This event should clear tracking data before a "normal" load can begin. Mods that circumvent the title screen should be accounted for with other events.
            FTMUtility.Helper.Events.GameLoop.SaveLoaded += (_, _) => LoadAndAddToWorld();

            FTMUtility.Helper.Events.GameLoop.Saving += (_, _) => SaveAndRemoveFromWorld();
            FTMUtility.Helper.Events.GameLoop.Saved += (_, _) => LoadAndAddToWorld();

            if (FTMUtility.ModAPIs.QuickSaveAPI != null)
            {
                FTMUtility.ModAPIs.QuickSaveAPI.LoadingEvent += (_, _) => Clear();
                FTMUtility.ModAPIs.QuickSaveAPI.LoadedEvent += (_, _) => LoadAndAddToWorld();

                FTMUtility.ModAPIs.QuickSaveAPI.SavingEvent += (_, _) => SaveAndRemoveFromWorld();
                FTMUtility.ModAPIs.QuickSaveAPI.SavedEvent += (_, _) => LoadAndAddToWorld();
            }

            //loot drops
        }

        /// <summary>Adds a new instance of this type to the tracking system.</summary>
        /// <param name="instance">The instance to track.</param>
        /// <param name="data">Data that describes this instance, which will be saved and later used to recreate the instance.</param>
        /// <param name="locationName">The instance's location's <see cref="GameLocation.NameOrUniqueName"/>.</param>
        public static void Add(Monster instance, MonsterData data, string locationName)
        {
            if (!Context.IsMainPlayer)
                throw new Exception("Failed to register a monster with the serializer. The current local player must be the host of a game session (StardewModdingAPI.Context.IsMainPlayer must be true).");
            else if (instance == null)
                throw new NullReferenceException($"Failed to register a monster with the serializer. The argument \"Monster instance\" is null.");
            else if (data == null)
                throw new NullReferenceException($"Failed to register a monster with the serializer. The argument \"MonsterData data\" is null.");
            else if (locationName == null)
                throw new NullReferenceException($"Failed to register a monster with the serializer. The argument \"string locationName\" is null.");

            if (!instance.modData.TryGetValue(FTMUtility.ModDataKeys.SerializerId, out string serializerId) || serializerId == null) //if this instance has no ID yet
            {
                //NOTE: reusing stored IDs is not strictly necessary, but provides a semi-permanent ID for each instance, which can be useful for debugging, other functions, other mods, etc
                serializerId = FTMUtility.Random.NextInt64().ToString(); //generate a new ID
                instance.modData[FTMUtility.ModDataKeys.SerializerId] = serializerId; //update it on the instance
            }

            int collisions = 0;
            while (IDsAndData.ContainsKey(serializerId)) //while this ID matches another tracked ID
            {
                serializerId = FTMUtility.Random.NextInt64().ToString();
                instance.modData[FTMUtility.ModDataKeys.SerializerId] = serializerId;

                collisions++;
                if (collisions >= 1000)
                    throw new Exception("Failed to generate a unique serializer ID; too many collisions. There may be a problem with the serializer code.");
            }

            IDsAndData.Add(serializerId, data);
            LocationNames.Add(locationName);
        }

        /// <summary>Gets stored data for an instance tracked by this serializer, if applicable.</summary>
        /// <param name="instance">The instance to check.</param>
        /// <returns>This instance's stored data. Null if the instance is not being tracked by this serializer.</returns>
        public static MonsterData GetData(Monster instance)
        {
            if (instance?.modData.TryGetValue(FTMUtility.ModDataKeys.SerializerId, out string serializerId) == true && IDsAndData.TryGetValue(serializerId, out MonsterData data)) //if this instance exists and has serialized data
                return data;

            return null;
        }

        /// <summary>Gets stored data for an instance tracked by this serializer, if applicable.</summary>
        /// <param name="serializerId">The serializer ID of the instance.</param>
        /// <returns>This instance's stored data. Null if the instance is not being tracked by this serializer.</returns>
        public static MonsterData GetData(string serializerId)
        {
            if (IDsAndData.TryGetValue(serializerId, out MonsterData data))
                return data;

            return null;
        }

        /// <summary>Removes a monster and its stored data from this serializer's tracking system, if applicable.</summary>
        /// <param name="instance">The instance to remove.</param>
        /// <remarks>
        /// <para>
        /// This method should only be called if the instance is no longer being used by the game, e.g. it's already been removed from an in-game location.
        /// </para>
        /// <para>
        /// This is not required to manage serialization; unused instances' IDs and data are automatically discarded during the save process.
        /// Removing unused instances "early" this way should slightly reduce memory usage, but at the cost of non-zero CPU usage when called.
        /// </para>
        /// </remarks>
        public static void Remove(Monster instance)
        {
            if (instance?.modData.TryGetValue(FTMUtility.ModDataKeys.SerializerId, out string serializerId) == true && IDsAndData.ContainsKey(serializerId)) //if this instance exists and has serialized data
                IDsAndData.Remove(serializerId);
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Clears all tracked instances and related information.</summary>
        private static void Clear()
        {
            IDsAndData.Clear();
            LocationNames.Clear();
        }

        /*****************/
        /* Event methods */
        /*****************/

        /// <summary>Finds tracked instances, removes them from the game, and creates save data for them. This method should be called before a game session starts saving.</summary>
        /// <remarks>
        /// <para>
        /// When this method ends, this class's save data should NOT be null, and instance tracking data should be cleared.
        /// </para>
        /// <para>
        /// If save data was NOT already null, this method should have no effect.
        /// This is intended to avoid errors if called multiple times before a single save (e.g. if a mod API event and normal event both trigger it).
        /// </para>
        /// </remarks>
        private static void SaveAndRemoveFromWorld()
        {
            if (!Context.IsMainPlayer)
                return;

            if (FTMUtility.Helper.Data.ReadSaveData<Dictionary<string, List<MonsterData>>>(SaveDataKey) != null) //if unloaded save data already exists (NOTE: the load process should null the data afterward)
                return;

            Dictionary<string, List<MonsterData>> saveDataByLocation = [];

            foreach (string locationName in LocationNames) //for each location with tracked instances
            {
                GameLocation location = FTMUtility.GetLocationIfActive(locationName);
                if (location == null)
                    continue;

                List<MonsterData> saveData = [];

                for (int x = location.characters.Count - 1; x >= 0; x--) //for each character at this location (looping backward for index-based removal)
                {
                    if (location.characters[x] is Monster monster && monster.modData.TryGetValue(FTMUtility.ModDataKeys.SerializerId, out string serializerId) && IDsAndData.TryGetValue(serializerId, out MonsterData data)) //if this is a monster with serialization data
                    {
                        MonsterManager.UpdateData(monster, data);
                        location.characters.RemoveAt(x);
                        saveData.Add(data); //add it to this location's save data   
                    }
                }

                saveDataByLocation[locationName] = saveData; //add this location's save data to the set
            }

            Clear();

            FTMUtility.Helper.Data.WriteSaveData(SaveDataKey, saveDataByLocation); //save the updated data set
        }

        /// <summary>Loads this serializer's save data, recreates each saved instance, and adds them to their previous in-game locations. This method should be called after a game session finishes loading or saving.</summary>
        /// <remarks>
        /// <para>
        /// When this method ends, this class's save data should be null, and the instance tracker should contain only loaded instances.
        /// </para>
        /// <para>
        /// If save data was already null (not just empty), this method should have no effect.
        /// This is intended to avoid errors if called multiple times after a single load or save (e.g. if a mod API event and normal event both trigger it).
        /// </para>
        /// </remarks>
        private static void LoadAndAddToWorld()
        {
            if (!Context.IsMainPlayer)
                return;

            var saveDataByLocation = FTMUtility.Helper.Data.ReadSaveData<Dictionary<string, List<MonsterData>>>(SaveDataKey); //get save data, or null if it doesn't exist
            if (saveDataByLocation == null) //if data is null (not just empty)
                return;

            foreach (var entry in saveDataByLocation) //for each location with saved instances
            {
                GameLocation location = FTMUtility.GetLocationIfActive(entry.Key);
                if (location == null)
                    continue;

                foreach (var saveData in entry.Value) //for each saved instance
                {
                    Monster monster = MonsterManager.Create(saveData, location);
                    if (monster == null)
                    {
                        FTMUtility.Monitor.LogOnce($"Failed to create and respawn a monster from save data (possibly due to removed mods). ID: \"{saveData?.SpawnId}\".", LogLevel.Trace);
                        continue;
                    }

                    monster.currentLocation = location;
                    location.addCharacter(monster);
                    Add(monster, saveData, entry.Key); //track the replaced instance
                }
            }

            FTMUtility.Helper.Data.WriteSaveData<Dictionary<string, List<MonsterData>>>(SaveDataKey, null); //null the save data
        }
    }
}
