using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "INDEX" tile query. Allows tiles that have certain index values on the specified layer.</summary>
    /// <remarks>Expected string format: "INDEX {Layer} [List of values]". Example: "INDEX Back 1 2 3".</remarks>
    public class IndexTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public IndexTileQuery(GameLocation location, string[] queryArgs)
        {
            Location = location;

            if (!ArgUtility.TryGet(queryArgs, 1, out string layer, out string error, false, "Map Layer Name"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            Layer = layer;

            if (queryArgs.Length > 2) //if any property values were provided
            {
                IndexValues = [];
                for (int x = 3; x < queryArgs.Length; x++)
                {
                    if (!ArgUtility.TryGetInt(queryArgs, x, out int index, out error, $"Tile index in argument {x}"))
                        throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");
                    IndexValues.Add(index);
                }
            }
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /// <summary>The name of the map layer to check.</summary>
        private string Layer { get; }

        /// <summary>A set of valid index values. Null if all values are valid.</summary>
        HashSet<int> IndexValues { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Normal;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => Location.getTileIndexAt((int)tile.X, (int)tile.Y, Layer) is int tileIndex && tileIndex >= 0 && (IndexValues == null || IndexValues.Contains(tileIndex)); //tile has an index on the layer (and, if specified, it matches)
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}