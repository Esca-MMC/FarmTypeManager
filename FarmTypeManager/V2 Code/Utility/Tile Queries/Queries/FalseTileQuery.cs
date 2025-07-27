using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "FALSE" tile query. Always returns false.</summary>
    public class FalseTileQuery : ITileQuery
    {
        /**************/
        /* ITileQuery */
        /**************/
        public int CheckTilePriority => ITileQuery.Priority_VeryHigh;
        public int StartingTilesPriority => ITileQuery.Priority_VeryHigh;
        public bool CheckTile(Vector2 tile) => false;
        public List<Vector2> GetStartingTiles() => []; //return an empty list
    }
}