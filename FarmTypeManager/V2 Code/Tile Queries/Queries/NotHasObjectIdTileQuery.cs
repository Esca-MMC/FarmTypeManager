using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!HAS_OBJECT_ID" tile query. Allows tiles that do NOT contain an object or big craftable with the specified ID(s).</summary>
    /// <remarks>Expected string format: "!HAS_OBJECT_ID {ID}+". Example: "!HAS_OBJECT_ID (O)16 (BC)9".</remarks>
    public class NotHasObjectIDTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotHasObjectIDTileQuery(GameLocation location, string[] queryArgs)
        {
            Location = location;
            Ids = new(StringComparer.OrdinalIgnoreCase);

            if (queryArgs.Length <= 1)
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: 'No ID arguments were provided'.");

            for (int x = 1; x < queryArgs.Length; x++)
                Ids.Add(queryArgs[x]);
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /// <summary>A set of qualified IDs to check. Case-insensitive.</summary>
        private HashSet<string> Ids { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Normal;
        public int StartingTilesPriority => ITileQuery.Priority_Normal;
        public bool CheckTile(Vector2 tile) => !(Location.Objects.TryGetValue(tile, out var obj) && Ids.Contains(obj.QualifiedItemId));
        public List<Vector2> GetStartingTiles()
        {
            List<Vector2> tiles = new();

            foreach (var entry in Location.Objects.Pairs)
                if (Ids.Contains(entry.Value.QualifiedItemId))
                    tiles.Add(entry.Key);

            return tiles;
        }
    }
}