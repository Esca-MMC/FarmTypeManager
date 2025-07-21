using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings used by <see cref="SpawnObjectHandler"/>.</summary>
    public class SpawnObjectSettings : ILocationSettings, ITileSettings, ISpawnItemSettings
    {
        /************************/
        /* Interface properties */
        /************************/
        public string Location { get; set; } = null;
        public List<string> LocationList { get; set; } = null;
        public string LocationListMode { get; set; } = "All";
        public string TileCondition { get; set; } = null;
        public FTMSpawnItemData Item { get; set; } = null;
        public List<FTMSpawnItemData> ItemList { get; set; } = null;
        public string ItemListMode { get; set; } = "Random";
        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<QuantityModifier> TimesModifiers { get; set; } = null;
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get; set; } = default;

        /********************/
        /* Other properties */
        /********************/
    }
}
