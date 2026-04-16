using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "SIZE_MAP" tile query. Allows tiles if every tile in a specified collision map is allowed by the sub-query.</summary>
    /// <remarks>Expected string format: "SIZE_MAP {collision map} {sub-query} [allow overlap]". Example: "SIZE_MAP XXX\nXOX\nXXX \"AREA 2 2 5 5, CAN_PLACE_ITEM\" true".</remarks>
    public class SizeMapTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public SizeMapTileQuery(GameLocation location, string[] queryArgs)
        {
            MapWidth = location.map.Layers[0].LayerWidth;
            MapHeight = location.map.Layers[0].LayerHeight;

            if (!ArgUtility.TryGet(queryArgs, 1, out string collisionMap, out string error, false, "string \"collision map\" in argument 1"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            if (!ArgUtility.TryGet(queryArgs, 2, out string subQuery, out error, false, "string \"sub-query\" in argument 2"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            if (!ArgUtility.TryGetOptionalBool(queryArgs, 3, out bool allowOverlap, out error, false, "bool \"allow overlap\" in argument 3"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            Queries = TileCondition.ParseQueries(location, subQuery);

            StartingTilesQuery = TileCondition.ChooseStartingTilesSource(Queries);

            CollisionTiles = FTMUtility.ParseCollisionMap(collisionMap); //parse the list of "impassable" tiles (e.g. 'X') in this map as offsets

            AllowOverlap = allowOverlap;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>Whether this multi-tile query should allow tiles to "overlap" between multiple <see cref="CheckTile(Vector2)"/> calls. Defaults to false.</summary>
        /// <remarks>
        /// <para>
        /// If false, when <see cref="CheckTile(Vector2)"/> returns "true", any tiles involved in the check will be marked "false" in <see cref="CheckTileCache"/>.
        /// </para>
        /// <para>
        /// For example, if a 2x2 tile area is checked and returns "true", all 4 of those tiles will be marked "false" for any additional checks by this instance.
        /// Setting this to "true" may cause this query to return tiles with overlapping multi-tile areas, depending on usage.
        /// </para>
        /// </remarks>
        private bool AllowOverlap { get; }

        /// <summary>The results of <see cref="CheckTile(Vector2)"/> for any tiles that were already checked. Used to skip redundant checks.</summary>
        private Dictionary<Vector2, bool> CheckTileCache { get; } = [];

        /// <summary><see cref="Location"/>'s height in tiles.</summary>
        private int MapHeight { get; }

        /// <summary><see cref="Location"/>'s width in tiles.</summary>
        private int MapWidth { get; }

        /// <summary>A list of sub-queries parsed from arguments.</summary>
        private List<ITileQuery> Queries { get; }

        /// <summary>The tile offsets to use when checking for collision.</summary>
        private List<Vector2> CollisionTiles { get; }

        /// <summary>The sub-query to use when getting a starting tile set, if any.</summary>
        private ITileQuery StartingTilesQuery { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_VeryLow;
        public int StartingTilesPriority => StartingTilesQuery?.StartingTilesPriority ?? ITileQuery.Priority_NotImplemented; //if an appropriate sub-query exists, use it; otherwise, treat this as not implemented
        public bool CheckTile(Vector2 tile) //using this tile as the starting position, check every tile in the area with every sub-query; return false if any return false
        {
            List<Vector2> tilesUsed = new(CollisionTiles.Count);

            foreach (Vector2 offset in CollisionTiles)
            {
                Vector2 tileToCheck = tile + offset; //get this tile of the collision map, relative to the provided tile

                if (CheckTileCache.TryGetValue(tileToCheck, out bool cachedResult))
                {
                    if (cachedResult)
                    {
                        tilesUsed.Add(tileToCheck);
                        continue; //this sub-tile is valid
                    }
                    else
                        return false; //this sub-tile is invalid, so the checked tile is invalid
                }

                if (tileToCheck.X < 0 || tileToCheck.X >= MapWidth || tileToCheck.Y < 0 || tileToCheck.Y >= MapHeight)
                {
                    CheckTileCache[tileToCheck] = false;
                    return false;
                }

                foreach (var query in Queries)
                {
                    if (!query.CheckTile(tileToCheck))
                    {
                        CheckTileCache[tileToCheck] = false;
                        return false;
                    }
                }

                tilesUsed.Add(tileToCheck);
                CheckTileCache[tileToCheck] = true;
            }

            //all tiles are valid

            if (AllowOverlap)
                foreach (Vector2 tileUsed in tilesUsed)
                    CheckTileCache.Remove(tileUsed); //re-check during future checks
            else
                foreach (Vector2 tileUsed in tilesUsed)
                    CheckTileCache[tileUsed] = false; //mark false for future checks

            return true;
        }
        public List<Vector2> GetStartingTiles() => StartingTilesQuery?.GetStartingTiles() ?? throw new NotImplementedException(); //if an appropriate sub-query exists, use it; otherwise, treat this as not implemented
    }
}