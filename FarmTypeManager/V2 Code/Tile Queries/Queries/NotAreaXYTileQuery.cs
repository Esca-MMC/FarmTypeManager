using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!AREA_XY" tile query. Allows tiles outside of a rectangular area between two tiles.</summary>
    /// <remarks>Expected string format: "!AREA_XY {X1} {Y1} {X2} {Y2}". Example: "!AREA_XY 2 2 5 5".</remarks>
    public class NotAreaXYTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotAreaXYTileQuery(string[] queryArgs)
        {
            if (!ArgUtility.TryGetPoint(queryArgs, 1, out Point tile1, out string error, "Area Tile 1")
                || !ArgUtility.TryGetPoint(queryArgs, 3, out Point tile2, out error, "Area Tile 2"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            TopLeftCorner = new(Math.Min(tile1.X, tile2.X), Math.Min(tile1.Y, tile2.Y)); //use the lower X and Y
            BottomRightCorner = new(Math.Max(tile1.X, tile2.X), Math.Max(tile1.Y, tile2.Y)); //use the higher X and Y
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The top left corner tile within the invalid area, i.e. the tile with the lowest X and Y values.</summary>
        private Point TopLeftCorner { get; set; }

        /// <summary>The bottom right corner tile within the invalid area, i.e. the tile with the highest X and Y values.</summary>
        private Point BottomRightCorner { get; set; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_High;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => tile.X < TopLeftCorner.X || tile.X > BottomRightCorner.X || tile.Y < TopLeftCorner.Y || tile.Y > BottomRightCorner.Y;
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}