using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A base for handlers that spawn a set of items at in-game locations.</summary>
    public abstract class SpawnItemHandlerBase<TSettings> : ICustomActionHandler where TSettings : class, ILocationSettings, ITileSettings, ISpawnItemSettings
    {
        /************************/
        /* ICustomActionHandler */
        /************************/

        public string ProviderModId => FTMUtility.Manifest?.UniqueID;
        public Type SettingsType => typeof(TSettings);
        public bool TryPerform(string actionType, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
        {
            var settings = rawSettings as TSettings;
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

            ItemQueryContext itemContext = new(queryContext.Location, queryContext.Player, FTMUtility.Random, $"FTM custom action handler. ActionType: \"{actionType}\". Trigger: \"{triggerContext.Trigger}\". Handler type: \"{typeof(SpawnObjectHandler)}\".");

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
                        if (!TrySpawnItems(locations[entry.Key], settings, queryContext, itemContext, entry.Value, out error))
                            return false;
                    }
                    break;
            }

            error = null;
            return true;
        }

        /****************/
        /* Base methods */
        /****************/

        /// <summary>Spawns a set number of items at a location.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="settings">The spawn settings to use.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="itemContext">The item context to use when generating items.</param>
        /// <param name="numberOfItems">The number of items to generate.</param>
        /// <param name="error">Error text describing why items could not be spawned. Null if no errors occurred.</param>
        /// <returns>True if spawning completed without errors, even if nothing was spawned (e.g. if tiles are blocked). False if any errors were encountered.</returns>
        protected virtual bool TrySpawnItems(GameLocation location, TSettings settings, GameStateQueryContext queryContext, ItemQueryContext itemContext, int numberOfItems, out string error)
        {
            queryContext = new(location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random, queryContext.IgnoreQueryKeys, queryContext.CustomFields); //use the current location for context

            var tileQuery = new TileCondition(location, settings.TileCondition);
            var tiles = tileQuery.GetTiles(true).GetEnumerator();

            int totalSpawned = 0;
            foreach (Item item in settings.CreateItems(queryContext, itemContext, numberOfItems, false))
            {
                if (!tiles.MoveNext()) //if no more tiles exist, stop
                    break;

                Vector2 tile = tiles.Current;

                if (TryPlaceItem(location, tile, item, out string placementError))
                {
                    totalSpawned++;
                }
                else if (placementError != null) //if placement failed and also returned error text
                {
                    error = $"Failed to spawn an item due to a placement error. Item ID: \"{item.QualifiedItemId}\". Error: \"{placementError}\".";
                    return false;
                }
            }

            if (totalSpawned > 0 || FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.Log($"Spawned {totalSpawned} items at {location.NameOrUniqueName}.", LogLevel.Trace);

            error = null;
            return true;
        }

        /// <summary>Places an item on a specified tile, if possible.</summary>
        /// <typeparam name="TItem">The type of <see cref="Item"/> being placed.</typeparam>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="tile">The tile to use.</param>
        /// <param name="item">The item to place.</param>
        /// <param name="error">Error text describing why the item was invalid. Null if the item was valid, even if it could not be placed due to obstructions.</param>
        /// <returns>True if the item was successfully placed. False if placement was obstructed, or if the item was invalid.</returns>
        /// <remarks>If error is null, this method should NOT cause the handler to return false; failure due to an obstructed tile is not an error. Only non-null error values (i.e. invalid items) should be handled as errors.</remarks>
        protected abstract bool TryPlaceItem<TItem>(GameLocation location, Vector2 tile, TItem item, out string error) where TItem : Item;
    }
}
