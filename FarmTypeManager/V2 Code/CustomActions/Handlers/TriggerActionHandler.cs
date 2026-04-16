using FarmTypeManager.Utilities;
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
        public string ProviderModId => Properties.Manifest?.UniqueID;
        public Type SettingsType => typeof(TriggerActionSettings);
        public bool TryPerform(string actionType, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
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
            int times = (int)Math.Round(Utility.ApplyQuantityModifiers(Properties.Random.Next(settings.MinTimes, settings.MaxTimes + 1), settings.TimesModifiers, settings.TimesModifierMode, queryContext.Location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, Properties.Random));
            if (times <= 0)
            {
                error = null;
                return true;
            }

            List<string> triggerActions = [];
            if (settings.Action != null)
                triggerActions.Add(settings.Action);
            if (settings.ActionList != null)
                triggerActions.AddRange(settings.ActionList);

            foreach (string action in Collections.SelectElementsByMode(triggerActions, settings.ActionListMode, times)) //select actions to perform
                if (!TriggerActionManager.TryRunAction(action, triggerContext.Trigger, triggerContext.TriggerArgs, out error, out _))
                    return false;

            error = null;
            return true;
        }
    }
}
