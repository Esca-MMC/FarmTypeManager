using Newtonsoft.Json;
using System.Collections.Generic;
using StardewValley;
using System;

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
        /// <remarks>
        ///     <para>Case-insensitive. Recognized values:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <term>All</term>
        ///             <description>Every action will be performed each time.</description>
        ///         </item>
        ///         <item>
        ///             <term>Random</term>
        ///             <description>One random action will be performed each time. Each action's chance is based on <see cref="CustomActionData.Weight"/>.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        public string ActionMode { get; init; } = "All";
        /// <summary>The minimum number of times to perform actions from this entry when it's triggered.</summary>
        /// <remarks>A random number between <see cref="MinTimes"/> and <see cref="MaxTimes"/> is chosen each time this entry is triggered.</remarks>
        public int MinTimes { get; init; } = 1;
        /// <summary>The maximum number of times to perform actions from this entry when it's triggered.</summary>
        /// <remarks>A random number between <see cref="MinTimes"/> and <see cref="MaxTimes"/> is chosen each time this entry is triggered.</remarks>
        public int MaxTimes { get; init; } = 1;
        /// <summary>A list of custom actions to perform when this entry is triggered.</summary>
        public Dictionary<string, CustomActionData> CustomActions { get; init; } = null;
    }
}
