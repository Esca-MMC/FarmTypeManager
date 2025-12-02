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
    /// If proper serialization is added for these types (e.g. via SpaceCore or future SMAPI support), the save method of this class can be safely disabled.
    /// Load events should temporarily stay enabled after such changes, so that players' saves still load any items that were saved.
    /// </para>
    /// </remarks>
    public static class PlacedItemSerializer
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The key to use for this class's save data.</summary>
        /// <remarks>This is intended for use with methods like <see cref="IDataHelper.WriteJsonFile"/>; those methods don't require keys to be unique between mods.</remarks>
        public static string SaveDataKey { get; set; } = "Serialization_PlacedItem";

        /// <summary>Set to true when saving begins, then false when loading ends.</summary>
        /// <remarks>Multiple "save" events, without "load" events in between them, would cause data loss. This flag is used to ignore any subsequent "save" events until a "load" happens.</remarks>
        private static bool CurrentlySaving { get; set; } = false;

        /// <summary>A set of all added instances to serialize, divided by their in-game location name. Keys are location names; values are lists of weak references to the instances.</summary>
        /// <remarks>These are sorted by location to allow efficient removal in some implementations. Weak references are used to allow instances to be removed by automatic garbage collection, e.g. if they're removed before save events.</remarks>
        private static Dictionary<string, List<WeakReference<PlacedItem>>> InstancesByLocation { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /***********/
        /* Methods */
        /***********/

        /// <summary>Initialize this serializer's events and other setup tasks.</summary>
        public static void Initialize()
        {
            FTMUtility.Helper.Events.GameLoop.Saving += (_, _) => SaveAndRemoveFromWorld();
            FTMUtility.Helper.Events.GameLoop.Saved += (_, _) => LoadAndAddToWorld();
            FTMUtility.Helper.Events.GameLoop.SaveLoaded += (_, _) => LoadAndAddToWorld();

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

        public static void Add(PlacedItem instance, string locationName)
        {
            if (!InstancesByLocation.TryGetValue(locationName, out var list)) //if this location has no list yet
            {
                list = new();
                InstancesByLocation.Add(locationName, list);
            }

            list.Add(new(instance)); //create and add a weak reference to the instance
        }

        public static void SaveAndRemoveFromWorld()
        {
            if (!Context.IsMainPlayer)
                return;

            CurrentlySaving = true;

            Dictionary<string, List<PlacedItemSaveData>> saveDataByLocation = new(StringComparer.OrdinalIgnoreCase);

            foreach (var entry in InstancesByLocation)
            {
                string locationName = entry.Key;
                GameLocation location = FTMUtility.GetLocationIfActive(locationName);
                if (location == null)
                    continue;

                List<PlacedItemSaveData> list = new();

                foreach (var reference in entry.Value)
                {
                    if (!reference.TryGetTarget(out PlacedItem instance))
                        continue;

                    Vector2 tile = instance.Tile;
                    if (location.terrainFeatures.TryGetValue(tile, out var existingInstance) && ReferenceEquals(instance, existingInstance)) //if the instance exists in-game at the correct tile
                    {
                        location.terrainFeatures.Remove(tile); //remove it from the location
                        list.Add(new(instance)); //create save data and add it to the location's list
                    }
                }

                if (list.Count > 0) //if any save data was created for this location
                    saveDataByLocation.Add(locationName, list); //add it to the save data set
            }

            FTMUtility.Helper.Data.WriteSaveData(SaveDataKey, saveDataByLocation);
            InstancesByLocation.Clear();
        }

        /// <summary>Load this serializer's save data, recreate each saved instance, and add it at its previous in-game location.</summary>
        /// <remarks>This method should be called after loading or saving a game session.</remarks>
        public static void LoadAndAddToWorld()
        {
            if (!Context.IsMainPlayer)
                return;

            //NOTE: don't clear tracked instances here; the save method should clear those when needed, and doing so here would cause problems with consecutive load calls

            var saveDataByLocation = FTMUtility.Helper.Data.ReadSaveData<Dictionary<string, List<PlacedItemSaveData>>>(SaveDataKey);
            if (saveDataByLocation == null || saveDataByLocation.Count < 1)
                return;

            foreach (var entry in saveDataByLocation)
            {
                GameLocation location = FTMUtility.GetLocationIfActive(entry.Key);
                if (location == null)
                    continue;

                foreach (var saveData in entry.Value)
                {
                    if (location.terrainFeatures.ContainsKey(saveData.Tile)) //if this instance's tile is already occupied
                        continue;

                    var instance = saveData.Create(); //recreate the saved instance

                    if (location.terrainFeatures.TryAdd(saveData.Tile, instance)) //NOTE: the instance's placement-related data (e.g. location and tile) is automatically updated by the game
                        Add(instance, entry.Key); //if placed successfully, add this instance to the tracker
                }
            }

            FTMUtility.Helper.Data.WriteSaveData<Dictionary<string, List<PlacedItemSaveData>>>(SaveDataKey, null); //clear save data
            CurrentlySaving = false;
        }
    }
}
