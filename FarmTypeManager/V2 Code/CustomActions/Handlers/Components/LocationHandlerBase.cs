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
    /// <summary>A base for handlers that perform actions at a customizable set of locations.</summary>
    public abstract class LocationHandlerBase<TSettings> : ICustomActionHandler where TSettings : class, ILocationSettings, ITimesToPerformSettings
    {
        /************************/
        /* ICustomActionHandler */
        /************************/

        public string ProviderModId => FTMUtility.Manifest?.UniqueID;
        public Type SettingsType => typeof(TSettings);
        public virtual bool TryPerform(string actionType, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
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
            int times = settings.GetRandomTimes(queryContext.Location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random);
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
                        if (!TryActionAtLocation(location, settings, queryContext, triggerContext, times, out error))
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
                        if (!TryActionAtLocation(locations[entry.Key], settings, queryContext, triggerContext, entry.Value, out error))
                            return false;
                    }
                    break;
            }

            error = null;
            return true;
        }

        /********************/
        /* Abstract methods */
        /********************/

        /// <summary>Tries to perform this action at the specified location.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="settings">The custom action's settings.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about a raised trigger.</param>
        /// <param name="numberOfItems">The number of items to generate.</param>
        /// <param name="error">Error text describing why items could not be spawned. Null if no errors occurred.</param>
        /// <returns>True if spawning completed without errors, even if nothing was spawned (e.g. if tiles are blocked). False if any errors were encountered.</returns>
        protected abstract bool TryActionAtLocation(GameLocation location, TSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfItems, out string error);
    }
}
