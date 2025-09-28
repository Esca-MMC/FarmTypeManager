using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns forage, craftables, or other basic placed <see cref="Object"/>s.</summary>
    public class SpawnObjectHandler : SpawnItemHandlerBase<SpawnItemSettings>
    {
        protected override string ModifyTileCondition(string tileCondition) => $"!HAS_OBJECT, {tileCondition}"; //exclude any tiles blocked by an existing object of this type

        protected override bool TryPlaceItem(GameLocation location, Vector2 tile, Item item, out string placementError)
        {
            if (item is Object obj && (obj.HasTypeObject() || obj.HasTypeBigCraftable()))
            {
                placementError = null;
                return location.objects.TryAdd(tile, obj);
            }
            else
            {
                placementError = $"This action can only place basic objects (O) and big craftables (BC).";
                return false;
            }
        }
    }
}
