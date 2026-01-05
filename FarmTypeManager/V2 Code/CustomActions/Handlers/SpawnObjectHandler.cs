using Microsoft.Xna.Framework;
using StardewValley;
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

        private static Object ModifyObjectForPlacement(Object obj, Vector2 tile)
        {
            if (obj is Torch torch) //if this is already a torch (as of SDV v1.6.15, "(O)" objects are generated this way)
            {
                torch.IsOn = true;
                torch.initializeLightSource(tile);
            }
            else if (obj.bigCraftable.Value && obj.HasContextTag("torch_item"))
            {
                if (!obj.modData.TryGetValue(FTMUtility.ModDataKeys.IsOn, out string isOnText) || !bool.TryParse(isOnText, out bool isOn)) //try to get this torch's on/off state from its mod data
                    isOn = true; //if mod data doesn't exist or parsing fails, default to true

                obj.modData.Remove(FTMUtility.ModDataKeys.IsOn); //if applicable, remove the data because it's now unnecessary (NOTE: disable this if a custom class with a pseudo-serializer can use the data)

                obj = new Torch(obj.ItemId, true) { IsOn = isOn }; //recreate the item as a torch, because BCs only use this class while placed
                obj.initializeLightSource(tile);
            }

            return obj;
        }
    }
}
