using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    /// <summary>A utility that provides a set of map tiles matching a set of conditions.</summary>
    public static class TileQueryManager
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>A set of raw tile queries and their parsed component conditions.</summary>
        private static Dictionary<string, List<ParsedTileCondition>> CachedQueryConditions { get; } = [];

        private static Dictionary<string, ITileConditionHandler> _tileConditionHandlers = null;
        /// <summary>A set of tile condition names and their associated handlers.</summary>
        private static Dictionary<string, ITileConditionHandler> TileConditionHandlers
        {
            get
            {
                if (_tileConditionHandlers == null)
                {
                    var basicHandler = new BasicTileConditionHandler(); //create a handler for all built-in conditions

                    _tileConditionHandlers = new(basicHandler.ConditionPriorities.Keys.Count, StringComparer.OrdinalIgnoreCase); //create a new case-insensitive dictionary with capacity = key count

                    foreach (string key in basicHandler.ConditionPriorities.Keys)
                        _tileConditionHandlers.Add(key, basicHandler); //associate each built-in condition key with this handler
                }

                return _tileConditionHandlers;
            }
        }

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Gets all tiles in this location that match this tile query.</summary>
        /// <param name="tileQuery">A tile query, i.e. a set of conditions and arguments, with each condition separated by commas.</param>
        /// <param name="location">The in-game location that the tile query should check. If null, no tiles will be returned.</param>
        /// <param name="tiles">A set of tiles to check. All values must be within the location's boundaries. If null, all map tiles will be returned in random order.</param>
        /// <returns>All tiles in the location that match the tile query, in random order.</returns>
        /// <remarks>This method validates and yields tiles one at a time. The results can only be iterated over once (e.g. in one foreach loop). Avoid converting them to an array or list, unless all valid tiles are needed.</remarks>
        public static IEnumerable<Vector2> GetTilesFromQuery(string tileQuery, GameLocation location, IEnumerable<Vector2> tiles = null)
        {
            if (location == null)
                yield break;

            if (tiles == null)
            {
                //get the maximum X and Y for tiles in this location
                int maxX = location.map.Layers[0].LayerWidth - 1;
                int maxY = location.map.Layers[0].LayerHeight - 1;

                tiles = GetTilesInRange(maxX, maxY, true); //get all map tiles in random order
            }

            var conditions = Parse(tileQuery); //parse the tile query into a set of conditions

            if (conditions == null) //if parsing failed
                yield break; //treat all tiles as non-matching

            conditions.Sort(); //sort conditions by priority

            foreach (var condition in conditions)
                tiles = condition.Handler.FilterTiles(condition.Args, location, tiles); //filter the tiles enumerator through this condition

            foreach (Vector2 tile in tiles) //get each tile through the nested enumerators (NOTE: each tile here will match all conditions; if no conditions were provided, all tiles match)
                yield return tile;
        }

        /// <summary>Gets a set of <see cref="Vector2"/> from 0,0 to X,Y.</summary>
        /// <param name="maximumX">The highest X value to generate (inclusive).</param>
        /// <param name="maximumY">The highest Y value to generate (inclusive).</param>
        /// <param name="randomOrder">If true, tiles will be returned in random order. If false, tiles will be returned from 0,0 to X,Y (incrementing Y first).</param>
        /// <returns>A set of <see cref="Vector2"/> between 0,0 and X,Y.</returns>
        public static IEnumerable<Vector2> GetTilesInRange(int maximumX, int maximumY, bool randomOrder = true)
        {
            if (randomOrder)
            {
                var tileList = new List<Vector2>((maximumX+1) * (maximumY+1));

                for (int x = 0; x <= maximumX; x++)
                {
                    for (int y = 0; y <= maximumY; y++)
                    {
                        tileList.Add(new Vector2(x, y));
                    }
                }

                ShuffleTileList(tileList);

                foreach (Vector2 tile in tileList)
                    yield return tile;
            }
            else
            {
                for (int x = 0; x <= maximumX; x++)
                {
                    for (int y = 0; y <= maximumY; y++)
                    {
                        yield return new Vector2(x, y);
                    }
                }
            }
        }

        /// <summary>Parse a tile query into individual conditions.</summary>
        /// <param name="tileQuery">A tile query, i.e. a set of conditions and arguments, with each condition separated by commas.</param>
        /// <returns>A list of parsed conditions, or null if parsing failed.</returns>
        public static List<ParsedTileCondition> Parse(string tileQuery)
        {
            if (string.IsNullOrWhiteSpace(tileQuery))
                tileQuery = "TRUE"; //treat all tiles as valid

            if (CachedQueryConditions.TryGetValue(tileQuery, out var cached)) //if this query has already been parsed
                return cached;

            //split query into single conditions by commas, but don't remove the quotes yet
            string[] conditions = ArgUtility.SplitQuoteAware(tileQuery, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, true);

            var parsed = new List<ParsedTileCondition>(conditions.Length);

            foreach (string condition in conditions)
            {
                string[] args = ArgUtility.SplitBySpaceQuoteAware(condition); //split condition into arguments around spaces (also removes empty entries)

                if (!TileConditionHandlers.TryGetValue(args[0], out var handler)) //if a handler does NOT exist for this condition key
                {
                    FTMUtility.Monitor.LogOnce($"Invalid tile query: \"{tileQuery}\". No tile condition named \"{args[0]}\" exists.", LogLevel.Warn);
                    return null;
                }

                parsed.Add(new ParsedTileCondition(args, handler)); //add the parsed information to the list
            }

            CachedQueryConditions[tileQuery] = parsed; //cache the parsed results
            return parsed;
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Randomize the order of items in a tile list.</summary>
        private static void ShuffleTileList(List<Vector2> tileList)
        {
            for (int current = tileList.Count - 1; current > 0; current--) //for each tile's index (counting backward and excluding 0)
            {
                int random = FTMUtility.Random.Next(current + 1); //get a random index between 0 and this tile's index
                
                //swap the current tile with the tile at the random index
                Vector2 temp = tileList[random];
                tileList[random] = tileList[current];
                tileList[current] = temp;
            }
        }
    }
}
