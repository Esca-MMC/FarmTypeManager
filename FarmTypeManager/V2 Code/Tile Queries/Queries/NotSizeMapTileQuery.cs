using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!SIZE_MAP" tile query. Rejects tiles if every tile in a specified collision map is allowed by the sub-query.</summary>
    /// <remarks>Expected string format: "!SIZE_MAP {collision map} {sub-query} [allow overlap]". Example: "!SIZE_MAP XXX\nXOX\nXXX \"AREA 2 2 5 5, CAN_PLACE_ITEM\" true".</remarks>
    public class NotSizeMapTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotSizeMapTileQuery(GameLocation location, string[] queryArgs)
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
        private SizeMapTileQuery QueryToInvert { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => QueryToInvert.CheckTilePriority;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !QueryToInvert.CheckTile(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}