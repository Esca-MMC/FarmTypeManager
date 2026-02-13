using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by handlers that spawn items, e.g. <see cref="SpawnObjectHandler"/>.</summary>
    public class SpawnItemSettings : ILocationSettings, ITileSettings, ISpawnItemSettings, ITimesToPerformSettings
    {
        /*********************/
        /* ILocationSettings */
        /*********************/

        public string Location { get; set; } = null;
        public List<string> LocationList { get; set; } = null;
        public SelectionMode LocationListMode { get; set; } = SelectionMode.All;

        /*****************/
        /* ITileSettings */
        /*****************/

        public string TileCondition { get; set; } = null;

        /**********************/
        /* ISpawnItemSettings */
        /**********************/

        public FTMSpawnItemData Item { get; set; } = null;
        public List<FTMSpawnItemData> ItemList { get; set; } = null;
        public SelectionMode ItemListMode { get; set; } = SelectionMode.Random;

        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;
    }
}
