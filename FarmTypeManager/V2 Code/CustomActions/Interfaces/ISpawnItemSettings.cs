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
        ItemListModes ItemListMode { get; set; }

        /// <summary>The available values of <see cref="ItemListMode"/>.</summary>
        public enum ItemListModes
        {
            /// <summary>All valid item data in this list should be used at once.</summary>
            /// <remarks>For example, if an action spawns objects and min/max times = 1, it should spawn an item from each entry with a valid condition.</remarks>
            All,
            /// <summary>Item data entries should be selected in random order.</summary>
            /// <remarks>For example, if an action spawns objects and min/max times = 1, it should spawn an item from a random valid entry.</remarks>
            Random
        }
    }
}
