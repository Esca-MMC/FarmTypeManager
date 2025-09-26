using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns forage, ore, or other placed <see cref="Object"/>s. This supports (O) and (BC) item types.</summary>
    public class SpawnObjectHandler : SpawnItemHandlerBase<SpawnObjectSettings>
    {
        /*******************/
        /* Private methods */
        /*******************/
        protected override bool TryPlaceItem<TItem>(GameLocation location, Vector2 tile, TItem item, out string error)
        {
            if (item is Object obj && (obj.HasTypeObject() || obj.HasTypeBigCraftable()))
            {
                error = null;
                return location.objects.TryAdd(tile, obj);
            }
            else
            {
                error = $"This action can only place basic objects (O) and big craftables (BC).";
                return false;
            }
        }
    }
}
