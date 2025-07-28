using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "SIZE" tile query. Allows tiles if every tile in a specified area is allowed by every sub-query.</summary>
    /// <remarks>Expected string format: "SIZE {X} {Y} {List of sub-queries}". Example: "SIZE 2 2 \"AREA 2 2 5 5\" \"CAN_PLACE_ITEM\"".</remarks>
    public class SizeTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        public SizeTileQuery(GameLocation location, string[] queryArgs)
        {
            MapWidth = location.map.Layers[0].LayerWidth;
            MapHeight = location.map.Layers[0].LayerHeight;

            if (!ArgUtility.TryGetInt(queryArgs, 1, out int sizeWidth, out string error, "Width")
                || !ArgUtility.TryGetInt(queryArgs, 2, out int sizeHeight, out error, "Height"))
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

            if (sizeWidth < 1 || sizeHeight < 1)
                throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: 'Width ({sizeWidth}) and Height ({sizeHeight}) must be above zero'.");

            SizeWidth = sizeWidth;
            SizeHeight = sizeHeight;

            //get any remaining arguments as queries, but require at least 1
            int x = 3;
            do
            {
                if (!ArgUtility.TryGet(queryArgs, x, out string subQuery, out error, false, $"Query in argument {x}"))
                    throw new ArgumentException($"The tile query '{string.Join(' ', queryArgs)}' couldn't be parsed. Reason: '{error}'.");

                string[] subQueryArgs = ArgUtility.SplitBySpaceQuoteAware(subQuery); //split sub-query into arguments around spaces (and remove empty entries)
                ITileQuery parsedSubQuery = TileCondition.TileQueryFactories[subQueryArgs[0]].CreateTileQuery(location, subQueryArgs); //create the sub-query (or throw an exception)
                Queries.Add(parsedSubQuery);

                x++;
            }
            while (x < queryArgs.Length);

            Queries.Sort((a, b) => b.CheckTilePriority.CompareTo(a.CheckTilePriority)); //sort by CheckTile priority from highest to lowest
            StartingTilesQuery = TileCondition.ChooseStartingTilesSource(Queries);
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary><see cref="Location"/>'s width in tiles.</summary>
        private int MapWidth { get; }

        /// <summary><see cref="Location"/>'s height in tiles.</summary>
        private int MapHeight { get; }

        /// <summary>A list of sub-queries parsed from arguments.</summary>
        private List<ITileQuery> Queries { get; set; } = [];

        /// <summary>The horizontal size of the area to check.</summary>
        private int SizeWidth { get; }

        /// <summary>The vertical size of the area to check.</summary>
        private int SizeHeight { get; }

        /// <summary>The sub-query to use when getting a starting tile set, if any.</summary>
        private ITileQuery StartingTilesQuery { get; set; }

        /// <summary>The results of <see cref="CheckTile(Vector2)"/> for any tiles that were already checked. Used to skip redundant checks.</summary>
        private Dictionary<Vector2, bool> CheckTileCache { get; set; } = [];

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_VeryLow;
        public int StartingTilesPriority => StartingTilesQuery?.StartingTilesPriority ?? ITileQuery.Priority_NotImplemented; //if an appropriate sub-query exists, use it; otherwise, treat this as not implemented
        public bool CheckTile(Vector2 tile) //using this tile as the top left corner, check every tile in the area with every sub-query; return false if any return false
        {
            List<Vector2> tilesUsed = new(SizeWidth * SizeHeight);

            for (int x = 0; x < SizeWidth; x++)
            {
                for (int y = 0; y < SizeHeight; y++)
                {
                    Vector2 tileToCheck = new(tile.X + x, tile.Y + y);

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

                    if (tileToCheck. X < 0 || tileToCheck.X >= MapWidth || tileToCheck.Y < 0 || tileToCheck.Y >= MapHeight)
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
            }

            //if all tiles were valid, cache them all as invalid now; this avoid overlap with future calls
            foreach (Vector2 tileUsed in tilesUsed)
                CheckTileCache[tileUsed] = false;

            return true;
        }
        public List<Vector2> GetStartingTiles() => StartingTilesQuery?.GetStartingTiles() ?? throw new NotImplementedException(); //if an appropriate sub-query exists, use it; otherwise, treat this as not implemented
    }
}