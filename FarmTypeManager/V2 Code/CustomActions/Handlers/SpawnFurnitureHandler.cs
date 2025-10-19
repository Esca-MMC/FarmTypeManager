using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns forage, craftables, or other basic placed <see cref="Object"/>s.</summary>
    public class SpawnFurnitureHandler : SpawnItemHandlerBase<SpawnItemSettings>
    {
        protected override bool TryPlaceItem(GameLocation location, Vector2 tile, Item item, out string placementError)
        {
            if (item is Furniture furniture)
            {
                furniture.Location = location;
                furniture.TileLocation = tile;
                location.furniture.Add(furniture);

                placementError = null;
                return true;
            }
            else
            {
                placementError = $"This action can only place furniture (F).";
                return false;
            }
        }

        protected override Vector2 GetItemSize(Item item)
        {
            if (item is Furniture furniture)
                return new Vector2(furniture.getTilesWide(), furniture.getTilesHigh());
            else
                return Vector2.One;
        }
    }
}
