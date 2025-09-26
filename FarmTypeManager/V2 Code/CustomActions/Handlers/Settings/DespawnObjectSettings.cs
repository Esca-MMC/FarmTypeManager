using StardewValley.GameData;
using System.Collections.Generic;
using static FarmTypeManager.CustomActions.ILocationSettings;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by <see cref="DespawnObjectHandler"/>.</summary>
    public class DespawnObjectSettings : ILocationSettings, ITileSettings, IDespawnItemSettings, ITimesToPerformSettings
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

        /************************/
        /* IDespawnItemSettings */
        /************************/
        public ItemMatchData ItemMatchData { get; set; } = null;
        public List<ItemMatchData> ItemMatchDataList { get; set; } = null;

        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;
    }
}
