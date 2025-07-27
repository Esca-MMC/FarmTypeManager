using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!AREA_WH" tile query. Allows tiles outside of the specified rectangular area.</summary>
    /// <remarks>Expected string format: "!AREA_WH {X} {Y} {Width} {Height}". Example: "!AREA_WH 2 2 5 5".</remarks>
    public class NotAreaWHTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotAreaWHTileQuery(string[] queryArgs)
        {
            if (!ArgUtility.TryGetRectangle(queryArgs, 1, out Rectangle rectangle, out string error, $"Area Rectangle"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");
            Rectangle = rectangle;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The rectangular area to consider invalid.</summary>
        private Rectangle Rectangle { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_High;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !Rectangle.Contains(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}