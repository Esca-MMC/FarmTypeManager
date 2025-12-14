using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "!AREA_MAP" tile query. Rejects tiles within a collision map, relative to the "zero" tile.</summary>
    /// <remarks>
    /// <para>
    /// Expected string format: "!AREA_MAP {x} {y} {collision map}". Example: "!AREA_MAP 2 2 XXX\nXOX\XXX".
    /// </para>
    /// <para>
    /// If a "zero" tile is specified in the collision map ('#' or '@'), that will be the target tile {X,Y}, and the map will be offset so that it's the "zero" tile on the map.
    /// If no "zero" tile is specified, the target tile will be the top left corner of the map.
    /// </para>
    /// <para>
    /// For example, "AREA_MAP 2 2 XXX\nX#X\nXXX" will allow tiles 1,1 to 3,3. "AREA_MAP 2 2 XXX\nXXX\nXXX" will allow tiles 2,2 to 4,4.
    /// </para>
    /// </remarks>
    public class NotAreaMapTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public NotAreaMapTileQuery(string[] queryArgs)
        {
            if (!ArgUtility.TryGetVector2(queryArgs, 1, out Vector2 targetTile, out string error, false, "Vector2 \"target tile\" in arguments 1-2")
                || !ArgUtility.TryGet(queryArgs, 3, out string collisionMap, out error, false, "string \"collision map\" in argument 3"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            var collisionTiles = FTMUtility.ParseCollisionMap(collisionMap); //get coordinates for each valid tile of the map, relative to the target tile

            foreach (Vector2 offset in collisionTiles)
                Tiles.Add(targetTile + offset); //add the actual tile represented by this map tile
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The tiles to reject.</summary>
        private HashSet<Vector2> Tiles { get; } = [];

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_High;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => !Tiles.Contains(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();
    }
}