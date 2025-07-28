using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "NOT" tile query. Allows tiles if they were rejected by its sub-query. Used to generically negate other queries, equivalent to the '!' prefix.</summary>
    /// <remarks>
    /// <para>Expected string format: "NOT {Sub-query}". Example: "NOT \"AREA 2 2 5 5\"".</para>
    /// <para>This query is mainly intended for use with custom queries that don't implement specific code for '!' versions. It's generally less efficient than directly implementing the negated query, and some queries (e.g. those with caching logic) may not be negated correctly.</para>
    /// </remarks>
    public class NotTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotTileQuery(GameLocation location, string[] queryArgs)
        {
            if (queryArgs.Length > 2)
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: 'Too many arguments were provided (expected 2, got {queryArgs.Length}). The sub-query argument may need to be wrapped in quotation marks. Example: NOT \"AREA 2 2 5 5\" '.");

            if (!ArgUtility.TryGet(queryArgs, 1, out string subQuery, out string error, false, "Query in argument 1"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            string[] subQueryArgs = ArgUtility.SplitBySpaceQuoteAware(subQuery); //split sub-query into arguments around spaces (and remove empty entries)
            ITileQuery parsedSubQuery = TileCondition.TileQueryFactories[subQueryArgs[0]].CreateTileQuery(location, subQueryArgs); //create the sub-query (or throw an exception)
            Query = parsedSubQuery;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The sub-query used to implement this query./summary>
        private ITileQuery Query { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => Query.CheckTilePriority;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !Query.CheckTile(tile); //negate the sub-query's result
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}