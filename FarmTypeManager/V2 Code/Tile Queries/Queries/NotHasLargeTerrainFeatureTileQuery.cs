using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!HAS_LARGE_TERRAIN_FEATURE" tile query. Allows tiles that do NOT intersect any large terrain features (see <see cref="GameLocation.largeTerrainFeatures"/>).</summary>
    public class NotHasLargeTerrainFeatureTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public NotHasLargeTerrainFeatureTileQuery(GameLocation location)
        {
            Location = location;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Low;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !TileHasLTF(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Checks whether any large terrain feature at this location exists on, or overlaps with, the given tile.</summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if any large terrain feature exists on the tile or overlaps with it.</returns>
        private bool TileHasLTF(Vector2 tile)
        {
            int x = (int)((tile.X + 0.5f) * 64f);
            int y = (int)((tile.Y + 0.5f) * 64f);
            Point position = new(x, y);

            foreach (LargeTerrainFeature ltf in Location.largeTerrainFeatures)
                if (ltf.getBoundingBox().Contains(position))
                    return true;

            return false;
        }
    }
}