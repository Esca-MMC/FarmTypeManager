using FarmTypeManager.Utilities;
using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by handlers that despawn (remove) items, e.g. <see cref="DespawnObjectHandler"/>.</summary>
    public class DespawnItemSettings : ILocationSettings, ITileSettings, IMatchItemSettings, ITimesToPerformSettings
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
