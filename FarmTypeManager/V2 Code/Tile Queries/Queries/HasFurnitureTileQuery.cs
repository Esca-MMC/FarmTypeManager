using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "HAS_FURNITURE" tile query. Allows tiles containing at least one furniture item.</summary>
    public class HasFurnitureTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public HasFurnitureTileQuery(GameLocation location)
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
        public bool CheckTile(Vector2 tile) => TileHasFurniture(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Checks whether any furniture at this location exists on, or overlaps with, the given tile.</summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if any furniture exists on the tile or overlaps with it.</returns>
        /// <remarks>This method is based on logic in <see cref="GameLocation.GetFurnitureAt(Vector2)"/>. Notably, this skips checking furniture's passable status.</remarks>
        private bool TileHasFurniture(Vector2 tile)
        {
            int x = (int)((tile.X + 0.5f) * 64f);
            int y = (int)((tile.Y + 0.5f) * 64f);
            Point position = new(x, y);

            foreach (Furniture f in Location.furniture)
                if (f.GetBoundingBox().Contains(position))
                    return true;

            return false;
        }
    }
}