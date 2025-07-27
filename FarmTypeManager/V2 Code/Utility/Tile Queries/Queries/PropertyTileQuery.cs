using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "PROPERTY" tile query. Allows tiles that have the specified tile property and optional value(s).</summary>
    /// <remarks>Expected string format: "PROPERTY {Layer} {Property} [List of values]". Example: "PROPERTY Back Diggable T True".</remarks>
    public class PropertyTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public PropertyTileQuery(GameLocation location, string[] queryArgs)
        {
            Location = location;

            if (!ArgUtility.TryGet(queryArgs, 1, out string layer, out string error, false, "Map Layer Name") ||
                !ArgUtility.TryGet(queryArgs, 2, out string propertyKey, out error, false, "Tile Property Key"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            Layer = layer;
            PropertyKey = propertyKey;

            if (queryArgs.Length > 3) //if any property values were provided
            {
                PropertyValues = new(StringComparer.OrdinalIgnoreCase);
                for (int x = 3; x < queryArgs.Length; x++)
                    PropertyValues.Add(queryArgs[x]);
            }
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /// <summary>The name of the map layer to check.</summary>
        private string Layer { get; }

        /// <summary>The key of the property to check.</summary>
        private string PropertyKey { get; }

        /// <summary>A set of valid property values. Case-insensitive. Null if all values are valid.</summary>
        HashSet<string> PropertyValues { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Normal;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => Location.doesTileHaveProperty((int)tile.X, (int)tile.Y, PropertyKey, Layer) is string propertyValue && (PropertyValues == null || PropertyValues.Contains(propertyValue)); //tile has the property and, if specified, a matching value
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}