using FarmTypeManager.TileQueries;
using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that despawns (removes) <see cref="Furniture"/>.</summary>
    public class DespawnFurnitureHandler : BasicHandlerBase<DespawnItemSettings>, ICustomActionHandler
    {
        protected override bool TryActionAtLocation(GameLocation location, DespawnItemSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int times, out string error)
        {
            List<ItemMatchData> matchData = new(settings.ItemMatchDataList ?? []);
            if (settings.ItemMatchData != null)
                matchData.Insert(0, settings.ItemMatchData);

            if (settings.TileCondition == null)
            {
                List<int> indexList = new(Enumerable.Range(0, location.furniture.Count));
                FTMUtility.RandomizeList(indexList);

                List<int> indicesToRemove = new();
                int count = location.furniture.Count;

                for (int x = 0; x < count; x++)
                {
                    if (times <= 0)
                        break;

                    if (location.furniture[x] is not Furniture furniture)
                        continue;

                    if (matchData.Count < 1 || matchData.Any((data) => data.Match(furniture, location, queryContext))) //if no match data was provided (i.e. everything matches) or if any data matches
                    {
                        indicesToRemove.Add(x);
                        times--;

                        if (FTMUtility.Monitor.IsVerbose)
                            FTMUtility.Monitor.VerboseLog($"{nameof(DespawnFurnitureHandler)}: Removing furniture. Location: \"{location.NameOrUniqueName}\". Tile: {furniture.TileLocation.X},{furniture.TileLocation.Y}. ID: \"{furniture.QualifiedItemId}\". Display Name: \"{furniture.DisplayName}\".");
                    }
                }

                for (int x = count - 1; x >= 0; x--) //loop backward (highest to lowest index numbers)
                    location.furniture.RemoveAt(x);
            }
            else
            {
                TileCondition tileCondition = new(location, $"HAS_FURNITURE, {settings.TileCondition}"); //create tile condition, limit it to tiles with objects
                var tiles = tileCondition.GetTiles();

                foreach (Vector2 tile in tiles)
                {
                    if (times <= 0)
                        break;

                    if (location.GetFurnitureAt(tile) is not Furniture furniture)
                        continue;

                    if (matchData.Count < 1 || matchData.Any((data) => data.Match(furniture, location, queryContext))) //if no match data was provided (i.e. everything matches) or if any data matches
                    {
                        location.furniture.Remove(furniture);
                        times--;

                        if (FTMUtility.Monitor.IsVerbose)
                            FTMUtility.Monitor.VerboseLog($"{nameof(DespawnFurnitureHandler)}: Removing furniture. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. ID: \"{furniture.QualifiedItemId}\". Display Name: \"{furniture.DisplayName}\".");
                    }
                }
            }

            error = null;
            return true;
        }
    }
}
