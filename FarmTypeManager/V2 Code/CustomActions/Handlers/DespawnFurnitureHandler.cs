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

            HashSet<Vector2> tilesToCheck; //a set of tiles matching the tile condition; null if no condition was used
            if (settings.TileCondition != null)
            {
                tilesToCheck = [];
                TileCondition tileCondition = new(location, settings.TileCondition); //NOTE: it'd be slower to add extra conditions for this type
                foreach (Vector2 tile in tileCondition.GetTiles())
                    tilesToCheck.Add(tile);
            }
            else
                tilesToCheck = null;

            List<int> indices = new(Enumerable.Range(0, location.furniture.Count));
            Collections.RandomizeList(indices);

            List<int> indicesToRemove = [];

            foreach (int index in indices)
            {
                if (times <= 0) //if enough instances matched already, stop looking for more
                    break;

                Furniture furniture = location.furniture[index];
                if (furniture == null)
                    continue;

                if (matchData.Count > 0 && matchData.Any(data => !data.Match(furniture, location, queryContext))) //if any data does NOT match this instance
                    continue;

                if (tilesToCheck != null && !SetContainsInstance(tilesToCheck, furniture)) //if this instance does NOT match the tile condition
                    continue;

                indicesToRemove.Add(index); //this instance matches, so mark it for removal
                times--;
            }

            indicesToRemove.Sort();
            for (int x = indicesToRemove.Count - 1; x >= 0; x--) //for each index to remove, looping backward to allow removal
            {
                int index = indicesToRemove[x];

                if (Properties.Monitor.IsVerbose)
                {
                    Furniture furniture = location.furniture[index];
                    if (furniture != null)
                        Properties.Monitor.VerboseLog($"{nameof(DespawnFurnitureHandler)}: Removing furniture. Location: \"{location.NameOrUniqueName}\". Tile: {furniture.TileLocation.X},{furniture.TileLocation.Y}. ID: \"{furniture.QualifiedItemId}\". Display Name: \"{furniture.DisplayName}\".");
                }

                location.furniture.RemoveAt(index);
            }

            error = null;
            return true;
        }

        /// <summary>Checks whether a set of tiles contains an in-game instance, based on its placement tile and size.</summary>
        /// <param name="tiles">The set of tiles to check.</param>
        /// <param name="instance">The instance to check.</param>
        /// <returns>True if the instance occupies any of the tiles in the set.</returns>
        private static bool SetContainsInstance(HashSet<Vector2> tiles, Furniture instance)
        {
            Vector2 placementTile = instance.TileLocation;
            int width = instance.getTilesWide();
            int height = instance.getTilesHigh();

            if (width <= 1 && height <= 1) //if this instance only occupies 1 tile
                return tiles.Contains(placementTile);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (tiles.Contains(new Vector2(placementTile.X + x, placementTile.Y + y))) //if the set contains any tile occupied by the instance
                        return true;

            return false;
        }
    }
}
