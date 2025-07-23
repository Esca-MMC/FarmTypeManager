using StardewValley;
using StardewValley.Internal;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="FTMSpawnItemData"/> data model.</summary>
    public static class FTMSpawnItemDataExtensions
    {
        /// <summary>Generate one random item from this data and apply this mod's additional settings.</summary>
        /// <inheritdoc cref="ItemQueryResolver.TryResolveRandomItem(StardewValley.GameData.ISpawnItemData, ItemQueryContext, bool, HashSet{string}, Func{string, string}, Item, Action{string, string})"/>
        public static Item TryResolveRandomItem(this FTMSpawnItemData data, ItemQueryContext context, bool avoidRepeat = false, HashSet<string> avoidItemIds = null, Func<string, string> formatItemId = null, Item inputItem = null, Action<string, string> logError = null)
        {
            Item item = ItemQueryResolver.TryResolveRandomItem(data, context, avoidRepeat, avoidItemIds, formatItemId, inputItem, logError);
            data.ApplyItemChanges(item);


            if (item is Chest chest)
            {
                foreach (FTMSpawnItemData containedItemData in data.Contents)
                {
                    if (containedItemData == null)
                        continue;

                    Item containedItem = containedItemData.TryResolveRandomItem(context, avoidRepeat, avoidItemIds, formatItemId, inputItem, logError);
                    if (containedItem != null)
                    {
                        containedItemData.ApplyItemChanges(containedItem);
                        chest.Items.Add(containedItem);
                    }
                }
            }

            //TODO: handle sub-items for other containers, e.g. a custom BreakableContainer; may require storing the items or data in a static manager, like 1.x loot drops

            return item;
        }

        /// <summary>Applies any necessary changes to a created item.</summary>
        /// <param name="item">The item to modify.</param>
        /// <returns>The modified item.</returns>
        private static void ApplyItemChanges(this FTMSpawnItemData data, Item item)
        {
            if (item == null)
                return;

            if (data.Indestructible == true)
                item.modData[FTMUtility.ModDataKeys.CanBePickedUp] = "false";

            if (item is Object obj)
            {
                string unqualifiedItemId = obj.ItemId;

                obj.IsSpawnedObject = data.CanPickUp ?? FTMUtility.CanPickUpByDefault(unqualifiedItemId);
                obj.Flipped = data.Flipped ?? obj.Flipped;
                obj.Fragility = data.Indestructible == true ? Object.fragility_Indestructable : data.Fragility ?? obj.Fragility; //override if data.Indestructible is true; otherwise, check data normally
                obj.MinutesUntilReady = data.Health ?? FTMUtility.GetDefaultObjectHealth(unqualifiedItemId) ?? obj.MinutesUntilReady;
            }
        }
    }
}
