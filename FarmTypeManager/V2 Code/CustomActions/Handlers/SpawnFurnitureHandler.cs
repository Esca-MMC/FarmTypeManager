using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns forage, craftables, or other basic placed <see cref="Object"/>s.</summary>
    public class SpawnFurnitureHandler : SpawnItemHandlerBase<SpawnItemSettings>
    {
        protected override string LogTextForInstances => "furniture";

        protected override string LogTextForAnInstance => "a furniture item";

        protected override Vector2 GetSize(Item instance)
        {
            if (instance is Furniture furniture)
                return new Vector2(furniture.getTilesWide(), furniture.getTilesHigh());
            else
                return Vector2.One;
        }

        protected override bool TrySpawn(GameLocation location, Vector2 tile, Item item, out string placementError)
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
                placementError = $"This action can only place furniture items. The item's type is \"{item?.GetType().FullName ?? "null"}\", which isn't a \"Furniture\" type.";
                return false;
            }
        }
    }
}
