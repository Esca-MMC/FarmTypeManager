using StardewModdingAPI;
using StardewValley.Delegates;
using System;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A handler only intended to test the custom action system.</summary>
    public class TestHandler : ICustomActionHandler
    {
        public string ProviderModId => FTMUtility.Manifest?.UniqueID;

        public Type SettingsType => typeof(TestSettings);

        public bool TryPerform(string actionId, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error)
        {
            var settings = rawSettings as TestSettings;

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

            FTMUtility.Monitor.Log($"Custom action \"Test\" was just performed! Arbitrary setting data: {settings.Fake_setting_3}", LogLevel.Info);
            error = null;
            return true;
        }
    }
}
