using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify item despawn (removal) data.</summary>
    public interface IDespawnItemSettings
    {
        /// <summary>An item matching data entry.</summary>
        ItemMatchData ItemMatchData { get; set; }

        /// <summary>A list of item matching data entries.</summary>
        List<ItemMatchData> ItemMatchDataList { get; set; }
    }
}
