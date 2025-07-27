using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "CAN_PLACE_ITEM" tile query. Allows tiles that are allowed by <see cref="GameLocation.CanItemBePlacedHere"/>.</summary>
    /// <param name="location">The in-game location to check.</param>
    public class CanPlaceItemTileQuery(GameLocation location) : ITileQuery
    {
        /********************/
        /* Class properties */
        /********************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; } = location;

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Low;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;

        public bool CheckTile(Vector2 tile) => Location.CanItemBePlacedHere(tile);
        public List<Vector2> GetStartingTiles() => throw new System.NotImplementedException();
    }
}