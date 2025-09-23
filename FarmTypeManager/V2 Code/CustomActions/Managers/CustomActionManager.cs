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
            { "DespawnObject", new DespawnObjectHandler() },
            { "SpawnObject", new SpawnObjectHandler() },
            { "TriggerAction", new TriggerActionHandler() }
        };

        /// <summary>A set of raw strings and the triggers parsed from them.</summary>
        private static Dictionary<string, HashSet<string>> ParsedTriggersCache { get; } = new(StringComparer.OrdinalIgnoreCase);

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
            if (FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.Log($"Custom action handler registered. Mod ID: \"{handler?.ProviderModId}\". Action ID: \"{actionId}\".", LogLevel.Trace);
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
                    if (entry.Value?.Trigger == null)
                        continue;

                    if (entry.Value.HostOnly && !Context.IsMainPlayer)
                        return;

                    if (entry.Value.MarkAppliedWithFlag != null && Game1.player.hasOrWillReceiveMail(entry.Value.MarkAppliedWithFlag)) //if this entry is already flagged as complete
                        continue;

                    if (!ParsedTriggersCache.TryGetValue(entry.Value.Trigger, out HashSet<string> parsedTriggers)) //try to get cached triggers for this trigger string; if it's not cached yet, parse and cache it
                    {
                        parsedTriggers = new(StringComparer.OrdinalIgnoreCase);
                        foreach (string trigger in entry.Value.Trigger.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                            parsedTriggers.Add(trigger);
                        ParsedTriggersCache.Add(entry.Value.Trigger, parsedTriggers);
                    }

                    if (!parsedTriggers.Contains(triggerContext.Trigger, StringComparer.OrdinalIgnoreCase)) //if this entry does NOT contain the specified trigger
                        continue;

                    if (entry.Value.Condition != null && !GameStateQuery.CheckConditions(entry.Value.Condition, queryContext)) //if this entry's condition is false
                        continue;

                    foreach (var action in GetActionsToPerform(asset.Item1, entry.Key, entry.Value, queryContext))
                    {
                        if (FTMUtility.Monitor.IsVerbose)
                            FTMUtility.Monitor.Log($"Performing a triggered custom action. Asset: \"{asset.Item1}\". Entry key: \"{entry.Key}\". Action key: \"{action.Key}\". Trigger: \"{triggerContext.Trigger}\".", LogLevel.Trace);

                        if (!TryPerformAction(action.Value, queryContext, triggerContext, out string error))
                        {
                            FTMUtility.Monitor.Log($"Failed to perform a custom action by trigger.", LogLevel.Warn);
                            FTMUtility.Monitor.Log($"Asset: \"{asset.Item1}\". Entry key: \"{entry.Key}\". Action key: \"{action.Key}\".", LogLevel.Warn);
                            FTMUtility.Monitor.Log($"Reason: {error}", LogLevel.Warn);
                        }

                        if (action.Value.MarkAppliedWithFlag != null)
                            Game1.player.mailReceived.Add(action.Value.MarkAppliedWithFlag); //add this action's completion flag
                    }

                    if (entry.Value.MarkAppliedWithFlag != null)
                        Game1.player.mailReceived.Add(entry.Value.MarkAppliedWithFlag); //add this entry's completion flag
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
                return;
            }

            if (entryData.HostOnly && !Context.IsMainPlayer)
                return;

            if (entryData.MarkAppliedWithFlag != null && Game1.player.hasOrWillReceiveMail(entryData.MarkAppliedWithFlag)) //if this entry is already flagged as complete
                return;

            if (entryData.Condition != null && !GameStateQuery.CheckConditions(entryData.Condition, queryContext)) //if this entry's condition is false
                return;

            foreach (var action in GetActionsToPerform(assetName, entryId, entryData, queryContext))
            {
                if (FTMUtility.Monitor.IsVerbose)
                    FTMUtility.Monitor.Log($"Performing a custom action by entry ID. Asset: \"{assetName}\". Entry key: \"{entryId}\". Action key: \"{action.Key}\".", LogLevel.Trace);

                if (!TryPerformAction(action.Value, queryContext, triggerContext, out string error))
                {
                    FTMUtility.Monitor.Log($"Failed to perform a custom action action by entry.", LogLevel.Warn);
                    FTMUtility.Monitor.Log($"Asset: \"{assetName}\". Entry key: \"{entryId}\". Action key: \"{action.Key}\".", LogLevel.Warn);
                    FTMUtility.Monitor.Log($"Reason: {error}", LogLevel.Warn);
                }

                if (action.Value.MarkAppliedWithFlag != null)
                    Game1.player.mailReceived.Add(action.Value.MarkAppliedWithFlag); //add this action's completion flag
            }

            if (entryData.MarkAppliedWithFlag != null)
                Game1.player.mailReceived.Add(entryData.MarkAppliedWithFlag); //add this entry's completion flag
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Gets a series of actions to perform from the given data when triggered. Keys are actions' string IDs, values are action data.</summary>
        /// <param name="assetId">A descriptive ID for this data's asset, e.g. the asset name used to load it through the content system.</param>
        /// <param name="entryId">The ID for this data within its asset.</param>
        /// <param name="data">The custom actions data to check for actions.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <returns>A series of actions to perform based on the current context, if any. Values </returns>
        private static IEnumerable<KeyValuePair<string, CustomActionData>> GetActionsToPerform(string assetId, string entryId, CustomActionsAssetEntry data, GameStateQueryContext queryContext)
        {
            if (data == null || data.CustomActions == null)
                yield break;

            if (data.MinTimes > data.MaxTimes)
            {
                FTMUtility.Monitor.Log($"Couldn't get custom actions from the asset \"{assetId}\", entry key \"{entryId}\". MinTimes ({data.MinTimes}) is greater than MaxTimes ({data.MaxTimes}).", LogLevel.Warn);
                yield break;
            }

            if (data.MaxTimes < 1)
                yield break;

            int timesToPerform;
            if (data.MinTimes == data.MaxTimes)
                timesToPerform = data.MinTimes;
            else
                timesToPerform = FTMUtility.Random.Next(data.MinTimes, data.MaxTimes + 1);

            if (timesToPerform < 1)
                yield break;

            int totalWeight = 0;
            List<KeyValuePair<string, CustomActionData>> actionList = new(data.CustomActions); //copy all actions
            foreach (var action in data.CustomActions)
            {
                if (action.Value == null)
                    actionList.Remove(action);
                else if (action.Value.Weight < 1)
                    actionList.Remove(action);
                else if (action.Value.MarkAppliedWithFlag != null && Game1.player.hasOrWillReceiveMail(action.Value.MarkAppliedWithFlag)) //if the action is already flagged as complete
                    actionList.Remove(action);
                else if (action.Value.Condition != null && !GameStateQuery.CheckConditions(action.Value.Condition, queryContext)) //if the action's condition is false
                    actionList.Remove(action);
                else
                    totalWeight += action.Value.Weight; //add up any valid list items' weights
            }

            switch (data.ActionMode)
            {
                case CustomActionsAssetEntry.ActionModes.Random:
                    for (int x = 0; x < timesToPerform; x++)
                    {
                        int random = FTMUtility.Random.Next(totalWeight);
                        foreach (var action in actionList)
                        {
                            if (random < action.Value.Weight)
                            {
                                yield return action;
                                break;
                            }
                            else
                                random -= action.Value.Weight;
                        }
                    }
                    break;
                case CustomActionsAssetEntry.ActionModes.All:
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
