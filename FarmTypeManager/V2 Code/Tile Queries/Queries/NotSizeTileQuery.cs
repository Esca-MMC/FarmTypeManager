using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!SIZE" tile query. Rejects tiles if every tile in a specified area is allowed by the sub-query.</summary>
    /// <remarks>Expected string format: "!SIZE {X} {Y} {Sub-query}". Example: "!SIZE 2 2 \"AREA 2 2 5 5, CAN_PLACE_ITEM\"".</remarks>
    public class NotSizeTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotSizeTileQuery(GameLocation location, string[] queryArgs)
        {
            QueryToInvert = new(location, queryArgs);
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The query that this "Not" query should invert.</summary>
        /// <remarks>
        /// This query creates a copy of another query, passes method calls to it, and inverts the results.
        /// This is generally used to avoid issues with unusual caching logic. A more efficient implementation could probably be developed.
        /// </remarks>
        private SizeTileQuery QueryToInvert { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => QueryToInvert.CheckTilePriority;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !QueryToInvert.CheckTile(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}