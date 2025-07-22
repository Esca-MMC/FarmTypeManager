using StardewValley.GameData;
using System.Collections.Generic;
using static FarmTypeManager.CustomActions.ILocationSettings;
using static FarmTypeManager.CustomActions.ISpawnItemSettings;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by <see cref="SpawnObjectHandler"/>.</summary>
    public class SpawnObjectSettings : ILocationSettings, ITileSettings, ISpawnItemSettings
    {
        /*********************/
        /* ILocationSettings */
        /*********************/

        public string Location { get; set; } = null;
        public List<string> LocationList { get; set; } = null;
        public LocationListModes LocationListMode { get; set; } = LocationListModes.All;

        /*****************/
        /* ITileSettings */
        /*****************/

        public string TileCondition { get; set; } = null;

        /**********************/
        /* ISpawnItemSettings */
        /**********************/

        public FTMSpawnItemData Item { get; set; } = null;
        public List<FTMSpawnItemData> ItemList { get; set; } = null;
        public ItemListModes ItemListMode { get; set; } = ItemListModes.Random;
        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;

        /********************/
        /* Other properties */
        /********************/
    }
}
