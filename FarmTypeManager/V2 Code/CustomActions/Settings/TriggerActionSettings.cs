using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by <see cref="TriggerActionHandler"/>.</summary>
    public class TriggerActionSettings : ITimesToPerformSettings
    {

        /// <summary>A trigger action string.</summary>
        public string Action { get; set; } = null;

        /// <summary>A list of trigger action strings.</summary>
        public List<string> ActionList { get; set; } = null;

        /// <summary>The behavior to use when selecting trigger actions.</summary>
        public SelectionMode ActionListMode { get; set; } = SelectionMode.All;

        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;
    }
}
