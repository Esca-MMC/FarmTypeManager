using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings with item matching data, e.g. for actions that modify or remove items.</summary>
    public interface IMatchItemSettings
    {
        /// <summary>An item matching data entry.</summary>
        ItemMatchData ItemMatchData { get; set; }

        /// <summary>A list of item matching data entries.</summary>
        List<ItemMatchData> ItemMatchDataList { get; set; }
    }
}
