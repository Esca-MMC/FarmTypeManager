using StardewValley.Delegates;
using System;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Implements and performs one or more custom actions.</summary>
    public interface ICustomActionHandler
    {
        /// <summary>The unique ID of the mod providing this handler.</summary>
        string ProviderModId { get; }

        /// <summary>The type of settings provided to the <see cref="TryPerform"/> method. Used to load and parse settings from assets.</summary>
        /// <remarks>This should generally be a custom class with fields used by a specific action ID. Vague field types like 'object' might limit Content Patcher's ability to add or edit settings.</remarks>
        Type SettingsType { get; }

        /// <summary>Try to perform the specified custom action with the given ID and settings.</summary>
        /// <param name="actionId">The custom action to perform.</param>
        /// <param name="rawSettings">The settings to use when performing the action, cast as a generic object.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about a raised trigger.</param>
        /// <param name="error">Error text describing why this action could not be performed, if applicable.</param>
        /// <returns>True if the custom action was successfully performed (even if nothing happened, e.g. due to settings or the game state). False if an error prevents the action.</returns>
        bool TryPerform(string actionId, object rawSettings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, out string error);
    }
}
