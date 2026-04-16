using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns forage, craftables, or other basic placed <see cref="Object"/>s.</summary>
    public class SpawnObjectHandler : SpawnItemHandlerBase<SpawnItemSettings>
    {
        /************************/
        /* SpawnItemHandlerBase */
        /************************/

        protected override string LogTextForAnInstance => "an object";

        protected override string LogTextForInstances => "objects";

        protected override string ModifyTileCondition(string tileCondition) => $"!HAS_OBJECT, {tileCondition}"; //exclude any tiles blocked by an existing object of this type

        protected override bool TrySpawn(GameLocation location, Vector2 tile, Item item, out string placementError)
        {
            if (item is Object obj)
            {
                obj = ModifyObjectForPlacement(obj, tile);
                obj.Location = location;
                obj.TileLocation = tile;

                placementError = null;
                return location.objects.TryAdd(tile, obj);
            }
            else
            {
                placementError = $"This action can only place basic objects. The item's type is \"{item?.GetType().FullName ?? "null"}\", which isn't an \"Object\" type.";
                return false;
            }
        }

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Applies any required modifications to an object before it's placed, e.g. to handle special object types or apply customizations from mod data.</summary>
        /// <param name="obj">The object to modify.</param>
        /// <param name="tile">The tile where the object will later be placed.</param>
        /// <returns>The modified object instance, or a replacement if its type needs to change before placement.</returns>
        private static Object ModifyObjectForPlacement(Object obj, Vector2 tile)
        {
            switch (obj)
            {
                case Torch torch: //if this is an "(O)" object torch (as of SDV v1.6.15, these have the type Torch in inventory, unlike "(BC)" torches)
                    torch.IsOn = true;
                    torch.initializeLightSource(tile);
                    break;

                default: //no type-based handling (e.g. it's just an Object)

                    if (obj.bigCraftable.Value)
                    {
                        if (obj.HasContextTag("torch_item"))
                        {
                            if (!obj.modData.TryGetValue(FTMUtility.ModDataKeys.IsOn, out string isOnText) || !bool.TryParse(isOnText, out bool isOn)) //try to get on/off state from mod data
                                isOn = true; //if not parsed, default to true
                            obj.modData.Remove(FTMUtility.ModDataKeys.IsOn); //if applicable, remove mod data (NOTE: disable this if the data is needed after spawn, e.g. if a pseudo-serializer handles these items)

                            Torch torch = new(obj.ItemId, true) { IsOn = isOn }; //recreate the item as a torch; BCs only use this class while placed
                            torch.modData.CopyFrom(obj.modData);
                            obj = torch;

                            torch.initializeLightSource(tile);
                        }
                        else if (obj.HasContextTag("sign_item"))
                        {
                            Sign sign = new(tile, obj.ItemId); //recreate the item as a sign; BCs only use this class while placed
                            sign.modData.CopyFrom(obj.modData);
                            obj = sign;
                        }
                    }

                    break;
            }
            return obj;
        }
    }
}
