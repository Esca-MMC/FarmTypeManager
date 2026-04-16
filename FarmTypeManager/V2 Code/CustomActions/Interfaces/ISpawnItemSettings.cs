using FarmTypeManager.Utilities;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify item spawn data.</summary>
    public interface ISpawnItemSettings
    {
        /// <summary>Spawnable item data. Used to create one or more items.</summary>
        FTMSpawnItemData Item { get; set; }

        /// <summary>A list of spawnable item data entries. Each can be used to create one or more items.</summary>
        List<FTMSpawnItemData> ItemList { get; set; }

        /// <summary>The behavior to use when selecting item data from <see cref="ItemList"/>.</summary>
        SelectionMode ItemListMode { get; set; }
    }
}
