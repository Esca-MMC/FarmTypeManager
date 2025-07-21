using StardewValley.GameData;

namespace FarmTypeManager.CustomActions
{
    /// <summary>An data model for spawnable items, with additions for this mod's features.</summary>
    public class FTMSpawnItemData : GenericSpawnItemDataWithCondition
    {
        /// <summary>The weight to use for this entry when randomly selecting it, if applicable.</summary>
        /// <remarks>This is equivalent to adding additional copies of this entry to a set. For example, an entry with weight = 10 should be 10 times as likely to be selected as an entry with weight = 1.</remarks>
        public int Weight { get; set; } = 1;

        /// <summary>The random chance that this entry should produce no items, from 0 (always produce items normally) to 1 (never produce items).</summary>
        public double ChanceToSkip { get; set; } = 0;
    }
}