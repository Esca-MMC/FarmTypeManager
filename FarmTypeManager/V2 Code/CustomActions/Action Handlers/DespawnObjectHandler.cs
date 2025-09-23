using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that despawns (removes) forage, ore, or other placed <see cref="Object"/>s.</summary>
    public class DespawnObjectHandler : ICustomActionHandler
    {
        /************************/
        /* ICustomActionHandler */
        /************************/

        public string ProviderModId => FTMUtility.Manifest?.UniqueID;
        public Type SettingsType => typeof(DespawnObjectSettings);
        public bool TryPerform(string actionId, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
        {
            var settings = rawSettings as DespawnObjectSettings;
            if (rawSettings == null)
            {
                error = "The provided settings are null.";
                return false;
            }
            else if (settings == null)
            {
                error = $"The provided settings are an unsupported type: \"{rawSettings.GetType()?.ToString() ?? "null"}\".";
                return false;
            }

            List<GameLocation> locations = settings.GetActiveLocations();
            if (locations.Count <= 0)
            {
                error = null;
                return true;
            }

            if (settings.MinTimes > settings.MaxTimes)
            {
                error = $"MinTimes ({settings.MinTimes}) is greater than MaxTimes ({settings.MaxTimes}).";
                return false;
            }

            //get a random number from min to max, apply modifiers, and round to the nearest integer
            int times = (int)Math.Round(Utility.ApplyQuantityModifiers(FTMUtility.Random.Next(settings.MinTimes, settings.MaxTimes + 1), settings.TimesModifiers, settings.TimesModifierMode, queryContext.Location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, FTMUtility.Random));
            if (times <= 0)
            {
                error = null;
                return true;
            }

            switch (settings.LocationListMode)
            {
                case ILocationSettings.LocationListModes.All:
                    foreach (GameLocation location in locations)
                    {
                        if (!TryRemoveItems(location, settings, queryContext, times, out error))
                            return false;
                    }
                    break;

                case ILocationSettings.LocationListModes.Random:
                default:
                    int count = locations.Count;
                    Dictionary<int, int> timesForEachLocationIndex = new(count); //key = random index in the location list; value = number of times to use that location
                    for (int x = 0; x < times; x++)
                    {
                        int index = FTMUtility.Random.Next(count);
                        if (timesForEachLocationIndex.ContainsKey(index))
                            timesForEachLocationIndex[index]++;
                        else
                            timesForEachLocationIndex[index] = 1;
                    }

                    foreach (var entry in timesForEachLocationIndex)
                    {
                        if (!TryRemoveItems(locations[entry.Key], settings, queryContext, entry.Value, out error))
                            return false;
                    }
                    break;
            }

            error = null;
            return true;
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Despawns (removes) a set number of <see cref="Object"/>s at a location.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="settings">The despawn settings to use.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="times">The number of matching items to remove.</param>
        /// <param name="error">Error text describing why items could not be spawned, if applicable.</param>
        private static bool TryRemoveItems(GameLocation location, DespawnObjectSettings settings, GameStateQueryContext queryContext, int times, out string error)
        {
            List<ItemMatchData> matchData = new(settings.ItemMatchDataList ?? []);
            if (settings.ItemMatchData != null)
                matchData.Insert(0, settings.ItemMatchData);

            var tileCondition = new TileCondition(location, $"HAS_OBJECT, {settings.TileCondition}"); //create tile condition, limit it to tiles with objects
            var tiles = tileCondition.GetTiles(true).GetEnumerator();

            for (int x = 0; x < times; x++)
            {
                if (!tiles.MoveNext()) //if no more tiles exist, stop
                    break;

                Vector2 tile = tiles.Current;

                if (!location.Objects.TryGetValue(tile, out Object obj) || obj is null) //try to get the object at this tile; if none exists, skip it
                    continue;

                if (matchData.Count < 1) //if no match data was provided, then all items match
                {
                    location.Objects.Remove(tile);

                    if (FTMUtility.Monitor.IsVerbose)
                        FTMUtility.Monitor.VerboseLog($"{nameof(DespawnObjectHandler)}: Removing a placed object. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. Object ID: \"{obj.QualifiedItemId}\".");
                }
                else
                {
                    foreach (var data in matchData)
                    {
                        if (data.Match(obj, location, queryContext))
                        {
                            location.Objects.Remove(tile);

                            if (FTMUtility.Monitor.IsVerbose)
                                FTMUtility.Monitor.VerboseLog($"{nameof(DespawnObjectHandler)}: Removing a placed object. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. Object ID: \"{obj.QualifiedItemId}\".");
                            break;
                        }
                    }
                }
            }

            error = null;
            return true;
        }

        /// <summary>Spawns a set number of <see cref="Object"/>s at a location.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="settings">The spawn settings to use.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="itemContext">The item context to use when generating items.</param>
        /// <param name="numberOfItems">The number of items to generate.</param>
        /// <param name="error">Error text describing why items could not be spawned, if applicable.</param>
        /// <returns>True if spawning completed without errors, even if nothing was spawned (e.g. if tiles are blocked). False if any errors were encountered.</returns>
        private static bool TrySpawnObjects(GameLocation location, SpawnObjectSettings settings, GameStateQueryContext queryContext, ItemQueryContext itemContext, int numberOfItems, out string error)
        {
            queryContext = new(location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random, queryContext.IgnoreQueryKeys, queryContext.CustomFields); //use the current location for context

            var tileQuery = new TileCondition(location, $"!HAS_OBJECT, {settings.TileCondition}"); //create tile condition, and require that the tile be clear of objects
            var tiles = tileQuery.GetTiles(true).GetEnumerator();

            int totalSpawned = 0;
            foreach (Item item in settings.CreateItems(queryContext, itemContext, numberOfItems, false))
            {
                if (!tiles.MoveNext()) //if no more tiles exist, stop
                    break;

                Vector2 tile = tiles.Current;

                if (item is Object obj && (obj.HasTypeObject() || obj.HasTypeBigCraftable()))
                {
                    if (location.objects.TryAdd(tile, obj))
                        totalSpawned++;
                }
                else if (item != null)
                {
                    error = $"Failed to spawn an object: this object type is not supported. Item ID: \"{item.QualifiedItemId}\".";
                    return false;
                }
            }

            if (totalSpawned > 0 || FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.Log($"Spawned {totalSpawned} objects at {location.NameOrUniqueName}.", LogLevel.Trace);

            error = null;
            return true;
        }
    }
}
