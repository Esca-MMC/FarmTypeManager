using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!SIZE" tile query. Allows tiles if any tile in a specified area is rejected by any sub-query.</summary>
    /// <remarks>Expected string format: "!SIZE {X} {Y} {List of sub-queries}". Example: "!SIZE 2 2 \"AREA 2 2 5 5\" \"CAN_PLACE_ITEM\"".</remarks>
    public class NotSizeTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotSizeTileQuery(GameLocation location, string[] queryArgs)
        {
            queryArgs[0] = queryArgs[0].Substring(1); //remove '!' from this query's key
            string queryString = string.Join(' ', queryArgs); //recombine the query into a string
            string[] subQueryArgs = ["NOT", queryString]; //create args for a "NOT" query with the new sub-query as an argument

            NotTileQuery query = (NotTileQuery)TileCondition.TileQueryFactories["NOT"].CreateTileQuery(location, subQueryArgs); //create the "NOT" query (or throw an exception)
            Query = query;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The sub-query used to implement this query./summary>
        private NotTileQuery Query { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_VeryLow;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !Query.CheckTile(tile); //use the opposite of the sub-query's result (NOTE: better implementations are possible, but low priority)
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}