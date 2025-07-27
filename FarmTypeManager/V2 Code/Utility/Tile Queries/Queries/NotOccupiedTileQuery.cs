using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!OCCUPIED" tile query. Allows tiles that are not occupied by objects or NPCs.</summary>
    public class NotOccupiedTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public NotOccupiedTileQuery(GameLocation location)
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
        public bool CheckTile(Vector2 tile) => !Location.IsTileOccupiedBy(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}