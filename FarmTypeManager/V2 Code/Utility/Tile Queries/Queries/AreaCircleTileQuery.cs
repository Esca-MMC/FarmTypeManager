using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "AREA_CIRCLE" tile query. Allows tiles within the specified radius of a tile or point.</summary>
    /// <remarks>Expected string format: "AREA_CIRCLE {X} {Y} {Radius}". Example: "AREA_CIRCLE 2 2 5".</remarks>
    public class AreaCircleTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public AreaCircleTileQuery(GameLocation location, string[] queryArgs)
        {
            int mapWidth = location.map.Layers[0].LayerWidth;
            int mapHeight = location.map.Layers[0].LayerHeight;

            if (!ArgUtility.TryGetVector2(queryArgs, 1, out Vector2 centerTile, out string error, false, "Center Tile")
                || !ArgUtility.TryGetInt(queryArgs, 3, out int radius, out error, "Radius"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            int minX = (int)centerTile.X - radius;
            int maxX = (int)Math.Ceiling(centerTile.X + radius);
            int minY = (int)centerTile.Y - radius;
            int maxY = (int)Math.Ceiling(centerTile.Y + radius);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight) //if the location has this tile
                    {
                        Vector2 tile = new(x, y);
                        if (Vector2.Distance(tile, centerTile) < radius) //if this tile is within range of the center
                            Tiles.Add(tile);
                    }
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The tiles to allow.</summary>
        private HashSet<Vector2> Tiles { get; } = [];

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_High;
        public int StartingTilesPriority => ITileQuery.Priority_High;
        public bool CheckTile(Vector2 tile) => Tiles.Contains(tile);
        public List<Vector2> GetStartingTiles() => Tiles.ToList();
    }
}