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
        /// <remarks>
        ///     <para>Case-insensitive. Recognized values:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <term>Random</term>
        ///             <description>Default. Item data entries should be selected in random order.</description>
        ///         </item>
        ///         <item>
        ///             <term>All</term>
        ///             <description>All item data from the list should be used at once. For example, an object-spawning action should spawn a copy of all generated objects.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        string ItemListMode { get; set; }

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
    }
}
