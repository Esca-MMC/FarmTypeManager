using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "KEY" tile query. Allows tiles that [...].</summary>
    /// <remarks>Expected string format: "KEY {arg1} {arg2}". Example: "KEY 1 2".</remarks>
    public class TEMPLATETileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public TEMPLATETileQuery(GameLocation location, string[] queryArgs)
        {
            Location = location;

            //parse any arguments like below, then set them in class properties (or if this has no args, remove queryArgs from the constructor + comment + class remarks)

            if (!ArgUtility.TryGet(queryArgs, 1, out string layer, out string error, false, "Map Layer Name"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Normal;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => false;
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}