using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that performs trigger actions.</summary>
    public class TriggerActionHandler : ICustomActionHandler
    {
        public string ProviderModId => FTMUtility.Manifest?.UniqueID;
        public Type SettingsType => typeof(TriggerActionSettings);
        public bool TryPerform(string actionId, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
        {
            var settings = rawSettings as TriggerActionSettings;

            if (settings == null)
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
            int times = (int)Math.Round(Utility.ApplyQuantityModifiers(FTMUtility.Random.Next(settings.MinTimes, settings.MaxTimes + 1), settings.TimesModifiers, settings.TimesModifierMode, queryContext.Location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, FTMUtility.Random));
            if (times <= 0)
            {
                error = null;
                return true;
            }

            List<string> triggerActions = new(settings.Actions ?? []);
            if (settings.Action != null)
                triggerActions.Insert(0, settings.Action);

            switch (settings.ActionsMode)
            {
                case TriggerActionSettings.ActionsModes.All:
                    for (int x = 0; x < times; x++)
                        foreach (string action in triggerActions)
                            if (!TriggerActionManager.TryRunAction(action, triggerContext.Trigger, triggerContext.TriggerArgs, out error, out _))
                                return false;
                    break;

                case TriggerActionSettings.ActionsModes.Random:
                default:
                    //create a list of random indices from the trigger list
                    List<int> randomIndexList = new(times);
                    int count = triggerActions.Count;
                    for (int x = 0; x < times; x++)
                        randomIndexList.Add(FTMUtility.Random.Next(count));

                    foreach (var index in randomIndexList) //run each randomly selected trigger
                    {
                        if (!TriggerActionManager.TryRunAction(triggerActions[index], triggerContext.Trigger, triggerContext.TriggerArgs, out error, out _))
                            return false;
                    }
                    break;
            }

            error = null;
            return true;
        }
    }
}
