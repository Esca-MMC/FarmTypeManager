using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!EXACT" tile query. Rejects one or more specific tiles.</summary>
    /// <remarks>Expected string format: "!EXACT {X Y}+". Example: "!EXACT 2 2".</remarks>
    public class NotExactTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotExactTileQuery(string[] queryArgs)
        {
            int x = 1;
            do
            {
                if (!ArgUtility.TryGetVector2(queryArgs, x, out Vector2 tile, out string error, true, $"Tile in argument {x}"))
                    throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

                Tiles.Add(tile);

                x += 2;
            }
            while (x < queryArgs.Length);
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The tiles to reject.</summary>
        private HashSet<Vector2> Tiles { get; } = [];

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_VeryHigh;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented; //probably marginally slower than the default full list
        public bool CheckTile(Vector2 tile) => !Tiles.Contains(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}