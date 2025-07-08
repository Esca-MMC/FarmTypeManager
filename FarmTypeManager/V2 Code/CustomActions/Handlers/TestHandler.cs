using FarmTypeManager.CustomActions;
using System;

namespace FarmTypeManager
{
    /// <summary>A handler only intended to test the custom action system.</summary>
    public class TestHandler : ICustomActionHandler
    {
        public string ModId => FTMUtility.Manifest?.UniqueID;

        public Type SettingsType => typeof(SpecificTestSettings);

        public bool TryPerform(string actionId, object rawSettings, out string error)
        {
            var settings = rawSettings as SpecificTestSettings;

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

            //FTMUtility.Monitor.Log($"Custom action \"Test\" was just performed! Arbitrary setting data: {settings.Fake_setting_3}", LogLevel.Info);
            string test = $"Custom action \"Test\" was just performed! Arbitrary setting data: {settings.Fake_setting_3}";
            _ = test.Trim();
            error = null;
            return true;
        }
    }
}
