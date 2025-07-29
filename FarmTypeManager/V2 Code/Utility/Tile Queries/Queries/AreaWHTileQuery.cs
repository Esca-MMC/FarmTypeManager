using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "AREA_WH" tile query. Allows tiles within the specified rectangular area.</summary>
    /// <remarks>Expected string format: "AREA_WH {X} {Y} {Width} {Height}". Example: "AREA_WH 2 2 5 5".</remarks>
    public class AreaWHTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public AreaWHTileQuery(GameLocation location, string[] queryArgs)
        {
            MapWidth = location.map.Layers[0].LayerWidth;
            MapHeight = location.map.Layers[0].LayerHeight;

            if (!ArgUtility.TryGetRectangle(queryArgs, 1, out Rectangle rectangle, out string error, $"Area Rectangle"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");
            Rectangle = rectangle;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The target location's height in tiles.</summary>
        private int MapHeight { get; }

        /// <summary>The target location's width in tiles.</summary>
        private int MapWidth { get; }

        /// <summary>The rectangular area to consider invalid.</summary>
        private Rectangle Rectangle { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_High;
        public int StartingTilesPriority => ITileQuery.Priority_High;
        public bool CheckTile(Vector2 tile) => Rectangle.Contains(tile);
        public List<Vector2> GetStartingTiles()
        {
            //Allow every tile from X,Y (inclusive) to X+Width,Y+Width (exclusive). Example: X=2, Y=2, Width=5, Height=5 should include tiles 2,2 through 6,6.

            List<Vector2> tiles = new(Rectangle.Width * Rectangle.Height);

            int right = Rectangle.Right;
            int bottom = Rectangle.Bottom;

            for (int x = Rectangle.X; x < right; x++)
                for (int y = Rectangle.Y; y < bottom; y++)
                    if (x >= 0 && x < MapWidth && y >= 0 && y < MapHeight) //if the location has this tile
                        tiles.Add(new Vector2(x, y));

            return tiles;
        }
    }
}