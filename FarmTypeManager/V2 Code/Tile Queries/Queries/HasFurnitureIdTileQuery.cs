using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "HAS_FURNITURE" tile query. Allows tiles containing at least one furniture item with the specified ID(s).</summary>
    /// <remarks>Expected string format: "HAS_FURNITURE_ID {id}+". Example: "HAS_FURNITURE_ID (F)6".</remarks>
    public class HasFurnitureIdTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public HasFurnitureIdTileQuery(GameLocation location, string[] queryArgs)
        {
            Location = location;
            Ids = new(StringComparer.OrdinalIgnoreCase);

            if (queryArgs.Length <= 1)
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: 'No ID arguments were provided'.");

            for (int x = 1; x < queryArgs.Length; x++)
                Ids.Add(queryArgs[x]);
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /// <summary>A set of qualified IDs to check. Case-insensitive.</summary>
        private HashSet<string> Ids { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Low;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => TileHasFurnitureWithId(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Checks whether any furniture at this location exists on, or overlaps with, the given tile, and also has one of the provided IDs.</summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if any furniture exists on the tile or overlaps with it, and also has one of the provided IDs.</returns>
        /// <remarks>This method is based on logic in <see cref="GameLocation.GetFurnitureAt(Vector2)"/>. Notably, this skips checking furniture's passable status.</remarks>
        private bool TileHasFurnitureWithId(Vector2 tile)
        {
            int x = (int)((tile.X + 0.5f) * 64f);
            int y = (int)((tile.Y + 0.5f) * 64f);
            Point position = new(x, y);

            foreach (Furniture f in Location.furniture)
                if (f.GetBoundingBox().Contains(position))
                    if (Ids == null || Ids.Contains(f.QualifiedItemId))
                        return true;

            return false;
        }
    }
}