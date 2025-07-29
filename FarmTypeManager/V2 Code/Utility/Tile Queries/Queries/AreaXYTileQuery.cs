using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using xTile.Dimensions;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "AREA_XY" tile query. Allows tiles within a rectangular area between two tiles.</summary>
    /// <remarks>Expected string format: "AREA_XY {X1} {Y1} {X2} {Y2}". Example: "AREA_XY 2 2 5 5".</remarks>
    public class AreaXYTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public AreaXYTileQuery(GameLocation location, string[] queryArgs)
        {
            MapWidth = location.map.Layers[0].LayerWidth;
            MapHeight = location.map.Layers[0].LayerHeight;

            if (!ArgUtility.TryGetPoint(queryArgs, 1, out Point tile1, out string error, "Area Tile 1")
                || !ArgUtility.TryGetPoint(queryArgs, 3, out Point tile2, out error, "Area Tile 2"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            TopLeftCorner = new(Math.Min(tile1.X, tile2.X), Math.Min(tile1.Y, tile2.Y)); //use the lower X and Y
            BottomRightCorner = new(Math.Max(tile1.X, tile2.X), Math.Max(tile1.Y, tile2.Y)); //use the higher X and Y
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The bottom right corner tile within the valid area, i.e. the tile with the highest X and Y values.</summary>
        private Point BottomRightCorner { get; set; }

        /// <summary>The target location's height in tiles.</summary>
        private int MapHeight { get; }

        /// <summary>The target location's width in tiles.</summary>
        private int MapWidth { get; }

        /// <summary>The top left corner tile within the valid area, i.e. the tile with the lowest X and Y values.</summary>
        private Point TopLeftCorner { get; set; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_High;
        public int StartingTilesPriority => ITileQuery.Priority_High;
        public bool CheckTile(Vector2 tile) => tile.X >= TopLeftCorner.X && tile.X <= BottomRightCorner.X && tile.Y >= TopLeftCorner.Y && tile.Y <= BottomRightCorner.Y;
        public List<Vector2> GetStartingTiles()
        {
            //Allow every tile from the top left corner to the bottom right corner (inclusive). Example: TopLeftCorner=(2,2), BottomRightCorner=(5,5) should include tiles 2,2 through 5,5.

            List<Vector2> tiles = new((1 + BottomRightCorner.X - TopLeftCorner.X) * (1 + BottomRightCorner.X - TopLeftCorner.X)); //pre-calculate the number of tiles: (1 + width) * (1 + height)

            for (int x = TopLeftCorner.X; x <= BottomRightCorner.X; x++)
                for (int y = TopLeftCorner.Y; y <= BottomRightCorner.Y; y++)
                    if (x >= 0 && x < MapWidth && y >= 0 && y < MapHeight) //if the location has this tile
                        tiles.Add(new Vector2(x, y));

            return tiles;
        }
    }
}