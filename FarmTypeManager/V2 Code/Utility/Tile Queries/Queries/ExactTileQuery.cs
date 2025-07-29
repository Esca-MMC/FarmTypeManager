using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "EXACT" tile query. Allows one or more specific tiles.</summary>
    /// <remarks>Expected string format: "EXACT {X Y}+". Example: "EXACT 2 2".</remarks>
    public class ExactTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public ExactTileQuery(GameLocation location, string[] queryArgs)
        {
            int mapWidth = location.map.Layers[0].LayerWidth;
            int mapHeight = location.map.Layers[0].LayerHeight;

            int x = 1;
            do
            {
                if (!ArgUtility.TryGetVector2(queryArgs, x, out Vector2 tile, out string error, true, $"Tile in argument {x}"))
                    throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

                if (tile.X >= 0 && tile.X < mapWidth && tile.Y >= 0 && tile.Y < mapHeight) //if the location has this tile
                    Tiles.Add(tile);

                x += 2;
            }
            while (x < queryArgs.Length);
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The tiles to allow.</summary>
        private HashSet<Vector2> Tiles { get; } = [];

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_VeryHigh;
        public int StartingTilesPriority => ITileQuery.Priority_VeryHigh;
        public bool CheckTile(Vector2 tile) => Tiles.Contains(tile);
        public List<Vector2> GetStartingTiles() => Tiles.ToList();
    }
}