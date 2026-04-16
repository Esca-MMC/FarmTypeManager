using FarmTypeManager.TileQueries;
using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Delegates;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that despawns (removes) forage, craftables, or other basic placed <see cref="Object"/>s.</summary>
    public class DespawnObjectHandler : BasicHandlerBase<DespawnItemSettings>, ICustomActionHandler
    {
        protected override bool TryActionAtLocation(GameLocation location, DespawnItemSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int times, out string error)
        {
            List<ItemMatchData> matchData = new(settings.ItemMatchDataList ?? []);
            if (settings.ItemMatchData != null)
                matchData.Insert(0, settings.ItemMatchData);

            IEnumerable<Vector2> tiles;

            if (settings.TileCondition == null)
            {
                List<Vector2> tileList = new(location.Objects.Keys);
                FTMUtility.RandomizeList(tileList);
                tiles = tileList;
            }
            else
            {
                TileCondition tileCondition = new(location, $"HAS_OBJECT, {settings.TileCondition}"); //create tile condition, limit it to tiles with objects
                tiles = tileCondition.GetTiles();
            }

            foreach (Vector2 tile in tiles)
            {
                if (times <= 0)
                    break;

                if (!location.Objects.TryGetValue(tile, out Object obj) || obj == null)
                    continue;

                if (matchData.Count < 1 || matchData.Any((data) => data.Match(obj, location, queryContext))) //if no match data was provided (i.e. everything matches) or if any data matches
                {
                    location.Objects.Remove(tile);
                    times--;

                    if (FTMUtility.Monitor.IsVerbose)
                        FTMUtility.Monitor.VerboseLog($"{nameof(DespawnObjectHandler)}: Removing a placed object. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. Object ID: \"{obj.QualifiedItemId}\".  Display Name: \"{obj.DisplayName}\".");
                }
            }

            error = null;
            return true;
        }
    }
}
