using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by <see cref="TriggerActionHandler"/>.</summary>
    public class TriggerActionSettings
    {
        /**********/
        /* Basics */
        /**********/

        /// <summary>A trigger action string.</summary>
        public string Action { get; set; } = null;

        /// <summary>A list of trigger action strings.</summary>
        public List<string> Actions { get; set; } = null;

        /**********/
        /* Extras */
        /**********/

        /// <summary>The behavior to use when selecting trigger actions.</summary>
        public ActionsModes ActionsMode { get; set; } = ActionsModes.All;

        /// <summary>The available values of <see cref="ActionsMode"/>.</summary>
        public enum ActionsModes
        {
            /// <summary>All trigger actions should be run.</summary>
            All,
            /// <summary>Trigger actions be used in random order.</summary>
            Random
        }

        /// <summary>The minimum number of times to run trigger actions.</summary>
        /// <remarks>When <see cref="ActionsMode"/> is set to "All", this is the minimum number of times to run each trigger action. Otherwise, this is the minimum number of random trigger actions to run.</remarks>
        public int MinTimes { get; set; } = 1;

        /// <summary>The maximum number of times to run trigger actions.</summary>
        /// <remarks>When <see cref="ActionsMode"/> is set to "All", this is the maximum number of times to run each trigger action. Otherwise, this is the maximum number of random trigger actions to run.</remarks>
        public int MaxTimes { get; set; } = 1;

        /// <summary>A list of modifiers to apply to the random number generated from <see cref="MinTimes"/> and <see cref="MaxTimes"/>.</summary>
        public List<QuantityModifier> TimesModifiers { get; set; } = null;

        /// <summary>The mode to use when combining modifiers from <see cref="TimesModifiers"/>.</summary>
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;
    }
}
