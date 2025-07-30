using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!ANY" tile query. Rejects tiles if any sub-query allows them.</summary>
    /// <remarks>Expected string format: "!ANY {List of sub-queries}". Example: "!ANY \"AREA 2 2 5 5\" \"AREA 12 2 5 5\"".</remarks>
    public class NotAnyTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotAnyTileQuery(GameLocation location, string[] queryArgs)
        {
            //get any arguments after the key as queries, but require at least 1
            int x = 1;
            do
            {
                if (!ArgUtility.TryGet(queryArgs, x, out string rawQuery, out string error, false, $"Query in argument {x}"))
                    throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

                if (TileCondition.ParseQueries(location, rawQuery) is var list && list.Count > 0)
                    Queries.Add(list);

                x++;
            }
            while (x < queryArgs.Length);

            Queries.Sort((a, b) => b[0].CheckTilePriority.CompareTo(a[0].CheckTilePriority)); //sort lists by the first entry's CheckTile priority from highest to lowest
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>A list of sub-queries, each parsed into its component queries. Each entry should be treated as true if all of its own sub-queries return true.</summary>
        private List<List<ITileQuery>> Queries { get; set; } = [];

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Low;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented; //this would require merging results from all sub-queries, which is slower and not always possible
        public bool CheckTile(Vector2 tile) => !Queries.Any((list) => list.All((query) => query.CheckTile(tile))); //false if ANY query's list of sub-queries is ALL true
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}