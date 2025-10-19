using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "HAS_RESOURCE_CLUMP" tile query. Allows tiles that intersect at least one resource clump (see <see cref="GameLocation.largeTerrainFeatures"/>). Notably includes giant crops.</summary>
    public class HasResourceClumpTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public HasResourceClumpTileQuery(GameLocation location)
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
        public bool CheckTile(Vector2 tile) => TileHasClump(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Checks whether any resource clump at this location exists on, or overlaps with, the given tile.</summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if any resource clump exists on the tile or overlaps with it.</returns>
        private bool TileHasClump(Vector2 tile)
        {
            int x = (int)((tile.X + 0.5f) * 64f);
            int y = (int)((tile.Y + 0.5f) * 64f);
            Point position = new(x, y);

            foreach (ResourceClump clump in Location.resourceClumps)
                if (clump.getBoundingBox().Contains(position))
                    return true;

            return false;
        }
    }
}