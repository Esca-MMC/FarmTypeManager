using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A handler only intended to test the custom action system.</summary>
    public class SpawnObjectHandler : ICustomActionHandler
    {
        /**********************/
        /* Interface features */
        /**********************/

        public string ProviderModId => FTMUtility.Manifest?.UniqueID;
        public Type SettingsType => typeof(SpawnObjectSettings);
        public bool TryPerform(string actionId, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
        {
            var settings = rawSettings as SpawnObjectSettings;
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

            ItemQueryContext itemContext = new(queryContext.Location, queryContext.Player, FTMUtility.Random, $"Creating items for FTM. Custom action ID: \"{actionId}\". Trigger ID: {triggerContext.Trigger}.");

            switch (settings.LocationListMode)
            {
                case ILocationSettings.LocationListModes.All:
                    foreach (GameLocation location in locations)
                    {
                        if (!TrySpawnItems(location, settings, queryContext, itemContext, times, out error))
                            return false;
                    }
                    break;

                case ILocationSettings.LocationListModes.Random:
                default:
                    int count = locations.Count;
                    Dictionary<int, int> timesForEachLocationIndex = new(count);
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
                        if (!TrySpawnItems(locations[entry.Key], settings, queryContext, itemContext, entry.Value, out error))
                            return false;
                    }
                    break;
            }

            error = null;
            return true;
        }

        /*******************/
        /* Implementations */
        /*******************/

        private bool TrySpawnItems(GameLocation location, SpawnObjectSettings settings, GameStateQueryContext queryContext, ItemQueryContext itemContext, int timesToRepeat, out string error)
        {
            queryContext = new(location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random, queryContext.IgnoreQueryKeys, queryContext.CustomFields); //use the current location for context

            List<Item> items = settings.CreateItems(queryContext, itemContext, timesToRepeat, false);
            Queue<Vector2> tiles = new(TileQueryManager.GetTilesFromQuery(settings.TileCondition, location).Take(items.Count)); //get enough tiles for each item, if possible

            int totalSpawned = 0;
            foreach (Item item in items)
            {
                if (tiles.Count == 0)
                    break;

                if (item is StardewValley.Object obj)
                {
                    if (location.objects.TryAdd(tiles.Dequeue(), obj))
                        totalSpawned++;
                }
            }

            FTMUtility.Monitor.Log($"Spawned {totalSpawned} objects at {location.NameOrUniqueName}.", LogLevel.Trace);

            error = null;
            return true;
        }
    }
}
