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
    /// This mod uses pseudo-serializers to save and load custom types that aren't recognized by the game's save seralizer.
    /// The game will throw errors and freeze if it attempts to save unrecognized in-game types.
    /// These pseudo-serializers remove instances before the game saves, save them elsewhere, and re-add them after the main save process.
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
        public static string SaveDataKey { get; set; } = "Serialization_PlacedItem";

        /// <summary>A set of all added instances to serialize, divided by their in-game location name. Keys are location names; values are lists of weak references to the instances.</summary>
        /// <remarks>
        /// These are sorted by location to allow efficient handling of some instance types.
        /// Weak references are used to allow instances to be removed by automatic garbage collection, e.g. if they're removed before save events.
        /// </remarks>
        private static Dictionary<string, List<WeakReference<PlacedItem>>> InstancesByLocation { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Initialize this serializer's events and other setup tasks.</summary>
        public static void Initialize()
        {
            FTMUtility.Helper.Events.GameLoop.Saving += (_, _) => SaveAndRemoveFromWorld();
            FTMUtility.Helper.Events.GameLoop.Saved += (_, _) => LoadAndAddToWorld();
            FTMUtility.Helper.Events.GameLoop.SaveLoaded += (_, _) => LoadAndAddToWorld();

            FTMUtility.Helper.Events.GameLoop.ReturnedToTitle += (_, _) => InstancesByLocation.Clear();

            if (FTMUtility.ModAPIs.QuickSaveAPI is var quickSave and not null) //NOTE: the APIs' method getters seem to trick "is" into allowing null, so "and not null" is necessary here
            {
                quickSave.SavingEvent += (_, _) => SaveAndRemoveFromWorld();
                quickSave.SavedEvent += (_, _) => LoadAndAddToWorld();
                quickSave.LoadedEvent += (_, _) => LoadAndAddToWorld();
            }

            if (FTMUtility.ModAPIs.SaveAnywhereAPI is var saveAnywhere and not null)
            {
                saveAnywhere.addBeforeSaveEvent(FTMUtility.Manifest.UniqueID, SaveAndRemoveFromWorld);
                saveAnywhere.addBeforeSaveEvent(FTMUtility.Manifest.UniqueID, LoadAndAddToWorld);
            }
        }

        /// <summary>Add a new instance of this type to the tracking system.</summary>
        /// <param name="instance">The instance to track.</param>
        /// <param name="locationName">The instance's location's <see cref="GameLocation.NameOrUniqueName"/>.</param>
        public static void Add(PlacedItem instance, string locationName)
        {
            if (!Context.IsMainPlayer)
                return;

            if (!InstancesByLocation.TryGetValue(locationName, out var list)) //if this location has no list yet
            {
                list = [];
                InstancesByLocation.Add(locationName, list);
            }

            list.Add(new(instance)); //add a weak reference to this instance
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

            InstancesByLocation.Clear(); //clear tracked instances
            
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
