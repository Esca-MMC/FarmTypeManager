using FarmTypeManager.Serialization;
using Microsoft.Xna.Framework;
using StardewValley;
using static FarmTypeManager.ModEntry;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns an item placed on the ground inside a custom subclass. Similar to <see cref="SpawnObjectHandler"/>, but implements support for more <see cref="Item"/> types and features.</summary>
    public class SpawnPlacedItemHandler : SpawnItemHandlerBase<SpawnItemSettings>
    {
        protected override string LogTextForAnInstance => "a placed item";

        protected override string LogTextForInstances => "placed items";

        protected override string ModifyTileCondition(string tileCondition) => $"!HAS_OBJECT, !HAS_SMALL_TERRAIN_FEATURE, {tileCondition}"; //exclude any tiles blocked by an existing object or terrain feature

        protected override bool TrySpawn(GameLocation location, Vector2 tile, Item item, out string placementError)
        {
            PlacedItem placed = new(item);

            if (item.modData.TryGetValue(FTMUtility.ModDataKeys.CanBePickedUp, out string val)) //if the contained item can't be picked up
                placed.modData[FTMUtility.ModDataKeys.CanBePickedUp] = val; //copy that setting to the container

            placementError = null;
            if (location.terrainFeatures.TryAdd(tile, placed))
            {
                PlacedItemSerializer.Add(placed, location.NameOrUniqueName);
                return true;
            }
            else
                return false;
        }
    }
}
