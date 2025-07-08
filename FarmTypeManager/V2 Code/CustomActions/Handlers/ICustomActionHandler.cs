using System;

namespace FarmTypeManager
{
    /// <summary>Implements and performs one or more custom actions.</summary>
    public interface ICustomActionHandler
    {
        /// <summary>The unique ID of the mod providing this handler.</summary>
        string ModId { get; }

        /// <summary>The type of settings provided to the <see cref="TryPerform"/> method. Used to load and parse settings from assets.</summary>
        /// <remarks>This should generally be a custom class with fields used by a specific action ID. Vague field types like 'object' might limit Content Patcher's ability to add or edit settings.</remarks>
        Type SettingsType { get; }

        /*
        /// <summary>Try to convert raw settings into a parsed instance for a specified action.</summary>
        /// <param name="actionId">The custom action these settings will be used to perform.</param>
        /// <param name="rawSettings">The collection of raw settings to parse.</param>
        /// <param name="parsedSettings">An instance containing parsed settings. This will cached and passed into "Perform" methods for this action.</param>
        /// <param name="error">Error text describing why these settings could not be parsed, if applicable.</param>
        /// <returns>True if the settings were successully parsed and output. False if the settings could not be parsed, e.g. if any settings are missing or invalid.</returns>
        bool TryParseSettings(string actionId, Dictionary<string, JToken> rawSettings, out object parsedSettings, out string error);
        */

        /// <summary>Try to perform the specified custom action with the given ID and settings.</summary>
        /// <param name="actionId">The custom action to perform.</param>
        /// <param name="rawSettings">The settings to use when performing the action, cast as a generic object.</param>
        /// <param name="error">Error text describing why this action could not be performed, if applicable.</param>
        /// <returns>True if the custom action was successfully performed (even if nothing happened, e.g. due to settings or the game state). False if an error prevents the action.</returns>
        bool TryPerform(string actionId, object rawSettings, out string error);
    }
}
