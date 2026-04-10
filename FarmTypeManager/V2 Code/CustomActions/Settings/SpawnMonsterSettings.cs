using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by <see cref="SpawnMonsterHandler"/>.</summary>
    public class SpawnMonsterSettings : ILocationSettings, ITileSettings, ITimesToPerformSettings
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

        /******************/
        /* New properties */
        /******************/

        public SpawnMonsterData Monster { get; set; } = null;
        public List<SpawnMonsterData> MonsterList { get; set; } = null;
        public SelectionMode MonsterListMode { get; set; } = SelectionMode.Random;
    }
}
