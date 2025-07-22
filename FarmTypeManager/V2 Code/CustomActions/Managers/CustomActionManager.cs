using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Provides methods to register and trigger custom actions.</summary>
    public static class CustomActionManager
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>A set of action IDs and the handlers used to perform them.</summary>
        private static Dictionary<string, ICustomActionHandler> Handlers { get; } = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Test", new TestHandler() },
            { "SpawnObject", new SpawnObjectHandler() }
        };

        /*******************/
        /* Handler methods */
        /*******************/

        /// <summary>Adds a new custom action ID and its implementation handler, or overwrites an existing action ID's handler.</summary>
        /// <param name="actionId">The custom action ID to handle. Case-insensitive.</param>
        /// <param name="handler">The instance that implements this action.</param>
        /// <remarks>To avoid issues with asset caching, other mods should provide handlers for all of their custom actions as soon as possible, e.g. in their Entry method or a GameLaunched event.</remarks>
        public static void RegisterCustomAction(string actionId, ICustomActionHandler handler)
        {
            Handlers[actionId] = handler;
            FTMUtility.Monitor.Log($"New custom action handler registered. Mod ID: \"{handler?.ProviderModId}\". Action ID: \"{actionId}\".", LogLevel.Trace);
        }

        /// <summary>Get the type of settings used by this custom action ID, if it exists.</summary>
        /// <param name="actionId">The custom action ID to check.</param>
        /// <returns>The type used for this custom action ID's settings, or null if the ID/handler/type doesn't exist.</returns>
        public static Type GetSettingsType(string actionId) => Handlers.TryGetValue(actionId, out var handler) ? handler?.SettingsType ?? null : null;

        /******************/
        /* Action methods */
        /******************/

        /// <summary>Perform custom actions from all entries in all assets, if they have the trigger specified in <paramref name="triggerContext"/>.</summary>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about the raised trigger.</param>
        public static void PerformActionsByTrigger(GameStateQueryContext queryContext, TriggerActionContext triggerContext)
        {
            foreach (var asset in CustomActionsAssetManager.GetAllData())
            {
                foreach (var entry in asset.Item2) //for each entry in this asset
                {
                    if (entry.Value?.Trigger != null && entry.Value.Trigger.Split(' ', System.StringSplitOptions.TrimEntries).Contains(triggerContext.Trigger, StringComparer.OrdinalIgnoreCase)) //if this entry contains the specified trigger
                    {
                        foreach (var action in GetActionsToPerform(asset.Item1, entry.Key, entry.Value, queryContext))
                        {
                            if (FTMUtility.Monitor.IsVerbose)
                                FTMUtility.Monitor.Log($"Performing a triggered custom action. Asset: \"{asset.Item1}\". Key: \"{entry.Key}\". Action: \"{action?.ActionId}\". Trigger: \"{triggerContext.Trigger}\".", LogLevel.Trace);

                            if (!TryPerformAction(action, queryContext, triggerContext, out string error))
                                FTMUtility.Monitor.Log($"Couldn't perform a custom action from the asset \"{asset.Item1}\", entry key \"{entry.Key}\". {error}", LogLevel.Warn);
                        }
                    }
                }
            }
        }

        /// <summary>Perform custom actions from a specific entry in a specific asset.</summary>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive.</param>
        /// <param name="entryId">The ID (key) of the entry within this asset.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about a raised trigger.</param>
        public static void PerformActionsFromEntry(string assetName, string entryId, GameStateQueryContext queryContext, TriggerActionContext triggerContext)
        {
            var asset = CustomActionsAssetManager.GetDataFromAsset(assetName);
            if (asset == null)
                return; //note: errors are handled in the asset method

            if (!asset.TryGetValue(entryId, out var entryData))
            {
                FTMUtility.Monitor.Log($"Couldn't get custom actions from the asset \"{assetName}\". The entry key \"{entryId}\" was not found.", LogLevel.Warn);
            }

            foreach (var action in GetActionsToPerform(assetName, entryId, entryData, queryContext))
            {
                if (FTMUtility.Monitor.IsVerbose)
                    FTMUtility.Monitor.Log($"Performing a custom action by entry ID. Asset: \"{assetName}\". Key: \"{entryId}\". Action: \"{action?.ActionId}\".", LogLevel.Trace);

                if (!TryPerformAction(action, queryContext, triggerContext, out string error))
                    FTMUtility.Monitor.Log($"Couldn't perform a custom action from the asset \"{assetName}\", entry key \"{entryId}\". {error}", LogLevel.Warn);
            }
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Gets a series of actions to perform from the given data when triggered.</summary>
        /// <param name="assetId">A descriptive ID for this data's asset, e.g. the asset name used to load it through the content system.</param>
        /// <param name="entryId">The ID for this data within its asset.</param>
        /// <param name="data">The custom actions data to check for actions.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <returns>A series of actions to perform, if any. This may vary between each call due to context, randomization, etc.</returns>
        private static IEnumerable<CustomActionData> GetActionsToPerform(string assetId, string entryId, CustomActionsAsset data, GameStateQueryContext queryContext)
        {
            if (data == null || data.CustomActions == null)
                yield break;

            if (data.MaxTimes < 1)
                yield break;

            if (data.MinTimes > data.MaxTimes)
            {
                FTMUtility.Monitor.Log($"Couldn't get custom actions from the asset \"{assetId}\", entry key \"{entryId}\". MinTimes ({data.MinTimes}) is greater than MaxTimes ({data.MaxTimes}).", LogLevel.Warn);
                yield break;
            }

            int timesToPerform;
            if (data.MinTimes == data.MaxTimes)
                timesToPerform = data.MinTimes;
            else
                timesToPerform = FTMUtility.Random.Next(data.MinTimes, data.MaxTimes + 1);

            if (timesToPerform < 1)
                yield break;

            if (data.Condition != null && !GameStateQuery.CheckConditions(data.Condition, queryContext)) //if the main condition is false
                yield break;

            int totalWeight = 0;
            List<CustomActionData> actionList = new(data.CustomActions.Values); //copy all actions
            foreach (var action in data.CustomActions.Values)
            {
                if (action.Weight < 1)
                    actionList.Remove(action);
                else if (action.Condition != null && !GameStateQuery.CheckConditions(data.Condition, queryContext)) //if the action's condition is false
                    actionList.Remove(action);
                else
                    totalWeight += action.Weight; //add up any valid list items' weights
            }

            switch (data.ActionMode)
            {
                case CustomActionsAsset.ActionModes.Random:
                    for (int x = 0; x < timesToPerform; x++)
                    {
                        int random = FTMUtility.Random.Next(totalWeight);
                        foreach (var action in actionList)
                        {
                            if (random < action.Weight)
                            {
                                yield return action;
                                break;
                            }
                            else
                                random -= action.Weight;
                        }
                    }
                    break;
                case CustomActionsAsset.ActionModes.All:
                default:
                    for (int x = 0; x < timesToPerform; x++)
                    {
                        foreach (var action in actionList)
                            yield return action;
                    }
                    break;
            }
        }

        /// <summary>Tries to perform a custom action with the given data.</summary>
        /// <param name="data">The data needed to perform a custom action.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about a raised trigger.</param>
        /// <param name="error">Error text describing why this action could not be performed, if applicable.</param>
        /// <returns>True if the custom action was successfully performed. False if it could not be performed, e.g. due to invalid settings.</returns>
        private static bool TryPerformAction(CustomActionData data, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
        {
            try
            {
                if (data.ActionId == null || !Handlers.TryGetValue(data.ActionId, out var handler) || handler == null)
                {
                    error = $"The custom action \"{data.ActionId}\" doesn't seem to exist.";
                    return false;
                }

                if (!handler.TryPerform(data.ActionId, data.Settings, queryContext, triggerContext, out string handlerError))
                {
                    error = $"The custom action \"{data.ActionId}\" failed: {handlerError}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                error = $"The custom action \"{data.ActionId}\" encountered an error:\n{ex}";
                return false;
            }

            if (FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.Log($"Successfully performed the custom action \"{data.ActionId}\".", LogLevel.Trace);

            error = "";
            return true;
        }
    }
}
