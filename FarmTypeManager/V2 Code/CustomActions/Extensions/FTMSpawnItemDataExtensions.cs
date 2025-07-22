using StardewValley;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="FTMSpawnItemData"/> data model.</summary>
    public static class FTMSpawnItemDataExtensions
    {
        /// <summary>Applies any necessary changes to a created item.</summary>
        /// <param name="item">The item to modify.</param>
        /// <returns>The modified item.</returns>
        public static void ApplyItemChanges(this FTMSpawnItemData data, Item item)
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
