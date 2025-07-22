using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify item spawn data.</summary>
    public interface ISpawnItemSettings
    {
        /// <summary>Spawnable item data. Used to create one or more items.</summary>
        FTMSpawnItemData Item { get; set; }

        /// <summary>A list of spawnable item data entries. Each can be used to create one or more items.</summary>
        List<FTMSpawnItemData> ItemList { get; set; }

        /// <summary>The behavior to use when selecting item data from <see cref="ItemList"/>.</summary>
        ItemListModes ItemListMode { get; set; }

        /// <summary>The minimum number of times to spawn items from this data.</summary>
        /// <remarks>When <see cref="ItemListMode"/> is set to "All", this is the minimum number of times to generate each item. Otherwise, this is the minimum number of items to generate.</remarks>
        int MinTimes { get; set; }

        /// <summary>The maximum number of times to spawn items from this data.</summary>
        /// <remarks>When <see cref="ItemListMode"/> is set to "All", this is the maximum number of times to generate each item. Otherwise, this is the maximum number of items to generate.</remarks>
        int MaxTimes { get; set; }

        /// <summary>A list of modifiers to apply to the random number generated from <see cref="MinTimes"/> and <see cref="MaxTimes"/>.</summary>
        List<QuantityModifier> TimesModifiers { get; set; }

        /// <summary>The mode to use when combining modifiers from <see cref="TimesModifiers"/>.</summary>
        QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; }

        /// <summary>The available values of <see cref="ILocationSettings.LocationListMode"/>.</summary>
        public enum ItemListModes
        {
            /// <summary>All valid item data in this list should be used at once.</summary>
            /// <remarks>For example, if an action spawns objects and min/max times = 1, it should spawn an item from each entry with a valid condition.</remarks>
            All,
            /// <summary>Item data entries should be selected in random order.</summary>
            /// <remarks>For example, if an action spawns objects and min/max times = 1, it should spawn an item from a random valid entry.</remarks>
            Random
        }
    }
}
