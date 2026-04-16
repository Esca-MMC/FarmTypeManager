using FarmTypeManager.Utilities;
using StardewValley;
using StardewValley.Delegates;
using System;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A base for most handlers. Performs one or more actions at one or more active locations.</summary>
    public abstract class BasicHandlerBase<TSettings> : ICustomActionHandler
        where TSettings : class, ILocationSettings, ITimesToPerformSettings
    {
        /************************/
        /* ICustomActionHandler */
        /************************/

        public string ProviderModId => Properties.Manifest?.UniqueID;
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

            if (settings.MinTimes > settings.MaxTimes)
            {
                error = $"MinTimes ({settings.MinTimes}) is greater than MaxTimes ({settings.MaxTimes}).";
                return false;
            }

            //get a random number from min to max, apply modifiers, and round to the nearest integer
            int times = settings.GetRandomTimes(queryContext);
            if (times <= 0)
            {
                error = null;
                return true;
            }

            foreach ((GameLocation, int) result in settings.GetActiveLocationsAndTimes(times)) //get active locations, and the number of times each location should be used
                if (!TryActionAtLocation(result.Item1, settings, queryContext, triggerContext, result.Item2, out error))
                    return false;

            error = null;
            return true;
        }

        /***************/
        /* New methods */
        /***************/

        /// <summary>Tries to perform this action at the specified location.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="settings">The custom action's settings.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about a raised trigger.</param>
        /// <param name="numberOfTimes">The number of times to perform the action.</param>
        /// <param name="error">Error text describing why the action couldn't be performed. Null if no errors occurred.</param>
        /// <returns>True if this method completed without errors, even the action had no effect (e.g. if item spawns were prevented by obstructions). False if any errors were encountered.</returns>
        protected abstract bool TryActionAtLocation(GameLocation location, TSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfItems, out string error);
    }
}
