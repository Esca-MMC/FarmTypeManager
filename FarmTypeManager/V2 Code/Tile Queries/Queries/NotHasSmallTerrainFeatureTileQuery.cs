using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!HAS_SMALL_TERRAIN_FEATURE" tile query. Allows tiles that do NOT have a small terrain feature (see <see cref="GameLocation.terrainFeatures"/>).</summary>
    public class NotHasSmallTerrainFeatureTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public NotHasSmallTerrainFeatureTileQuery(GameLocation location)
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
        public bool CheckTile(Vector2 tile) => !Location.terrainFeatures.ContainsKey(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}