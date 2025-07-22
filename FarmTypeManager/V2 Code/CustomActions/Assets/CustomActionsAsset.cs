using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of data that describes when to trigger customizable actions, which actions to trigger, and settings specific to each action.</summary>
    public class CustomActionsAsset
    {
        /// <summary>A set of triggers from the Data/TriggerActions system. When one of these triggers occurs, this entry's actions will be performed.</summary>
        /// <remarks>Case-insensitive. Multiple triggers should be separated by spaces.</remarks>
        public string Trigger { get; init; } = null;
        /// <summary>A <see cref="GameStateQuery"/> condition. If it's null or it returns true when checked, this entry's actions can be performed.</summary>
        public string Condition { get; init; } = null;
        /// <summary>The method to use when selecting actions to perform from <see cref="CustomActions"/>.</summary>
        public ActionModes ActionMode { get; init; } = ActionModes.All;
        /// <summary>The minimum number of times to perform actions from this entry when it's triggered.</summary>
        /// <remarks>A random number between <see cref="MinTimes"/> and <see cref="MaxTimes"/> is chosen each time this entry is triggered.</remarks>
        public int MinTimes { get; init; } = 1;
        /// <summary>The maximum number of times to perform actions from this entry when it's triggered.</summary>
        /// <remarks>A random number between <see cref="MinTimes"/> and <see cref="MaxTimes"/> is chosen each time this entry is triggered.</remarks>
        public int MaxTimes { get; init; } = 1;
        /// <summary>A list of custom actions to perform when this entry is triggered.</summary>
        public Dictionary<string, CustomActionData> CustomActions { get; init; } = null;

        /// <summary>The available values of <see cref="ActionMode"/>.</summary>
        public enum ActionModes
        {
            /// <summary>Every valid action will be performed each time.</summary>
            All,
            /// <summary>One random action will be performed each time, with chances based on <see cref="CustomActionData.Weight"/>.</summary>
            Random
        }
    }
}
