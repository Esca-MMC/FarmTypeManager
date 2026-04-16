using FarmTypeManager.Utilities;
using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A data model for item creation, for use as a field or property type.</summary>
    public class ItemSpawnField : ITimesToPerformSettings, ISpawnItemSettings
    {
        /**********************/
        /* ISpawnItemSettings */
        /**********************/

        public FTMSpawnItemData Item { get; set; } = null;
        public List<FTMSpawnItemData> ItemList { get; set; } = null;
        public SelectionMode ItemListMode { get; set; } = SelectionMode.All;

        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;
    }
}
