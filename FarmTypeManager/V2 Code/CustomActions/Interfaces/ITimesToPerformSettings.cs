using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify the number of times to perform the associated action(s).</summary>
    public interface ITimesToPerformSettings
    {
        /// <summary>The minimum number of times to perform this action.</summary>
        int MinTimes { get; set; }

        /// <summary>The maximum number of times to perform this action.</summary>
        int MaxTimes { get; set; }

        /// <summary>A list of modifiers to apply to the random number generated from <see cref="MinTimes"/> and <see cref="MaxTimes"/>.</summary>
        List<QuantityModifier> TimesModifiers { get; set; }

        /// <summary>The mode to use when combining modifiers from <see cref="TimesModifiers"/>.</summary>
        QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; }
    }
}
