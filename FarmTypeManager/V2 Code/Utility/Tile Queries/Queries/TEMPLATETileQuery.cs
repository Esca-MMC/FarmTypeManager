using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "[KEY]" tile query. Allows tiles that [...].</summary>
    /// <remarks>Expected string format: "!AREA {X} {Y} {Width} {Height}". Example: "!AREA 1 1 5 5".</remarks>
    public class TEMPLATETileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public TEMPLATETileQuery(GameLocation location)
        {
            Location = location;
        }

        /********************/
        /* Class properties */
        /********************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /**************/
        /* ITileQuery */
        /**************/
        public int CheckTilePriority => ITileQuery.Priority_VeryHigh;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => false;
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}