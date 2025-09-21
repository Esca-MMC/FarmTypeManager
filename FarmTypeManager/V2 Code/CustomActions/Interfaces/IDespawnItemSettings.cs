using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify item despawn (removal) data.</summary>
    public interface IDespawnItemSettings
    {
        /// <summary>An item matching data entry.</summary>
        ItemMatchData ItemMatchData { get; set; }

        /// <summary>A list of item matching data entries.</summary>
        List<ItemMatchData> ItemMatchDataList { get; set; }

        /// <summary>The minimum number of items to remove.</summary>
        int MinTimes { get; set; }

        /// <summary>The maximum number of items to remove.</summary>
        int MaxTimes { get; set; }

        /// <summary>A list of modifiers to apply to the random number generated from <see cref="MinTimes"/> and <see cref="MaxTimes"/>.</summary>
        List<QuantityModifier> TimesModifiers { get; set; }

        /// <summary>The mode to use when combining modifiers from <see cref="TimesModifiers"/>.</summary>
        QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; }
    }
}
