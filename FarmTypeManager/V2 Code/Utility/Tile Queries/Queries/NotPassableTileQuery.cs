using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!PASSABLE" tile query. Allows tiles on which players and NPCs cannot generally move.</summary>
    public class NotPassableTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public NotPassableTileQuery(GameLocation location)
        {
            Location = location;
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
        public bool CheckTile(Vector2 tile) => !Location.isTilePassable(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}