using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "TRUE" tile query. Always returns true.</summary>
    public class TrueTileQuery : ITileQuery
    {
        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_VeryHigh;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => true;
        public List<Vector2> GetStartingTiles() => throw new System.NotImplementedException();
    }
}