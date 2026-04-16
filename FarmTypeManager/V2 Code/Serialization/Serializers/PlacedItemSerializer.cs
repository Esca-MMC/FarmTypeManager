using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using static FarmTypeManager.ModEntry;

namespace FarmTypeManager.Serialization
{
    /// <summary>The pseudo-serializer used to save and load <see cref="PlacedItem"/> instances.</summary>
    /// <remarks>
    /// <para>
    /// This mod uses workaround serializers to save and load custom types that aren't recognized by the game's save seralizer.
    /// The game will throw errors and freeze if it attempts to save unrecognized in-game types.
    /// These serializers remove instances before the game saves, save them elsewhere, and re-add them after the main save process.
    /// </para>
    /// <para>
    /// If proper serialization is added for these types (e.g. via SpaceCore or future SMAPI support), this class's "save" method(s) should be safe to disable.
    /// Other features should stay enabled after such changes, so that players' saves still load any items that were saved.
    /// </para>
    /// <para>
    /// As of this writing, this serialization method cannot account for instances that move between different <see cref="GameLocation"/>s.
    /// </para>
    /// </remarks>
    public static class PlacedItemSerializer
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The key to use for this class's save data.</summary>
        /// <remarks>This is intended for use with methods like <see cref="IDataHelper.WriteJsonFile"/>, which don't require keys to be unique between mods.</remarks>
        public static string SaveDataKey { get; set; } = "PlacedItemSerializer";

        /// <summary>A set of all added instances to serialize, divided by their in-game location name. Keys are location names; values are lists of weak references to the instances.</summary>
        /// <remarks>
        /// These are sorted by location to allow efficient handling of some instance types.
        /// Weak references are used to allow instances to be removed by automatic garbage collection, e.g. if they're removed before save events.
        /// </remarks>
        private static Dictionary<string, List<WeakReference<PlacedItem>>> InstancesByLocation { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /*****************/
        /* Setup methods */
        /*****************/

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
            FTMUtility.Helper.Events.GameLoop.ReturnedToTitle += (_, _) => InstancesByLocation.Clear(); //NOTE: this is equivalent to clearing before a load can begin
            FTMUtility.Helper.Events.GameLoop.SaveLoaded += (_, _) => LoadAndAddToWorld();

            FTMUtility.Helper.Events.GameLoop.Saving += (_, _) => SaveAndRemoveFromWorld();
            FTMUtility.Helper.Events.GameLoop.Saved += (_, _) => LoadAndAddToWorld();

            if (FTMUtility.ModAPIs.QuickSaveAPI != null)
            {
                FTMUtility.ModAPIs.QuickSaveAPI.LoadingEvent += (_, _) => InstancesByLocation.Clear();
                FTMUtility.ModAPIs.QuickSaveAPI.LoadedEvent += (_, _) => LoadAndAddToWorld();

                FTMUtility.ModAPIs.QuickSaveAPI.SavingEvent += (_, _) => SaveAndRemoveFromWorld();
                FTMUtility.ModAPIs.QuickSaveAPI.SavedEvent += (_, _) => LoadAndAddToWorld();
            }
        }

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Adds a new instance of this type to the tracking system.</summary>
        /// <param name="instance">The instance to track.</param>
        /// <param name="locationName">The instance's location's <see cref="GameLocation.NameOrUniqueName"/>.</param>
        public static void Add(PlacedItem instance, string locationName)
        {
            if (!Context.IsMainPlayer)
                throw new Exception("Failed to register a placed item with the serializer. The current local player must be the host of a game session (StardewModdingAPI.Context.IsMainPlayer must be true).");
            else if (instance == null)
                throw new NullReferenceException($"Failed to register a placed item with the serializer. The argument \"PlacedItem instance\" is null.");
            else if (locationName == null)
                throw new NullReferenceException($"Failed to register a placed item with the serializer. The argument \"string locationName\" is null.");

            if (!InstancesByLocation.TryGetValue(locationName, out var list)) //if this location has no list yet
            {
                list = [];
                InstancesByLocation.Add(locationName, list);
            }

            list.Add(new(instance)); //add a weak reference to this instance
        }

        /*******************/
        /* Private methods */
        /*******************/

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

            if (FTMUtility.Helper.Data.ReadSaveData<Dictionary<string, List<PlacedItemSaveData>>>(SaveDataKey) != null) //if unloaded save data already exists (NOTE: the load process should null the data afterward)
                return;

            Dictionary<string, List<PlacedItemSaveData>> saveDataByLocation = [];

            foreach (var entry in InstancesByLocation) //for each location with tracked instances
            {
                string locationName = entry.Key;
                GameLocation location = FTMUtility.GetLocationIfActive(locationName);
                if (location == null)
                    continue;

                List<PlacedItemSaveData> saveData = [];

                foreach (var instanceReference in entry.Value) //for each instance at this location
                {
                    if (!instanceReference.TryGetTarget(out PlacedItem instance)) //if it no longer exists at all, skip it
                        continue;

                    Vector2 tile = instance.Tile;
                    if (location.terrainFeatures.TryGetValue(tile, out var instanceOnThisTile) == true && ReferenceEquals(instance, instanceOnThisTile)) //if the instance exists in-game at the correct tile
                    {
                        location.terrainFeatures.Remove(tile); //remove it from the game
                        saveData.Add(new(instance)); //create save data for it
                    }
                }

                saveDataByLocation[locationName] = saveData; //add this location's save data to the set
            }

            InstancesByLocation.Clear(); //clear tracked instances

            FTMUtility.Helper.Data.WriteSaveData(SaveDataKey, saveDataByLocation); //save updated data
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

            var saveDataByLocation = FTMUtility.Helper.Data.ReadSaveData<Dictionary<string, List<PlacedItemSaveData>>>(SaveDataKey); //read save data, if any
            if (saveDataByLocation == null) //if data is null (not just empty)
                return;

            foreach (var entry in saveDataByLocation) //for each location with saved instances
            {
                GameLocation location = FTMUtility.GetLocationIfActive(entry.Key);
                if (location == null)
                    continue;

                foreach (var saveData in entry.Value) //for each saved instance
                {
                    if (location.terrainFeatures.ContainsKey(saveData.Tile)) //if the instance's tile is already occupied
                        continue;

                    var instance = saveData.Create(); //recreate the instance

                    if (location.terrainFeatures.TryAdd(saveData.Tile, instance)) //try to place it (NOTE: an instance's Location and Tile are automatically set when placed)
                        Add(instance, entry.Key); //if the instance was placed, track it
                }
            }

            FTMUtility.Helper.Data.WriteSaveData<Dictionary<string, List<PlacedItemSaveData>>>(SaveDataKey, null); //null the save data
        }
    }
}
