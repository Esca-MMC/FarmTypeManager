using FarmTypeManager.Utilities;
using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by handlers that despawn (remove) monsters.</summary>
    public class DespawnMonsterSettings : ILocationSettings, ITileSettings, ITimesToPerformSettings
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

        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;

        /********************/
        /* Other properties */
        /********************/

        /// <summary>An monster matching data entry.</summary>
        public MonsterMatchData MonsterMatchData { get; set; } = null;

        /// <summary>A list of monster matching data entries.</summary>
        public List<MonsterMatchData> MonsterMatchDataList { get; set; } = null;
    }
}
