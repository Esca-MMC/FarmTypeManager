using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A parsed tile condition. Outputs tiles that are considered valid by all queries in a tile condition string.</summary>
    public class TileCondition
    {
        /****************/
        /* Constructors */
        /****************/

        /// <summary>Create a new tile condition for the given location and tile condition.</summary>
        /// <param name="location">The in-game location to check.</param>
        /// <param name="condition">A set of tile queries and their arguments, separated by commas.</param>
        public TileCondition(GameLocation location, string condition)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Condition = condition;

            Initialize();
        }

        //TODO: add a constructor that specifies a size
        //      convert the condition from "query 1, query 2, etc" into "SIZE X Y \"query 1\" \"query 2\" \"etc\""

        /**************/
        /* Properties */
        /**************/

        /// <summary>Tile query keys and the factories that create their tile queries.</summary>
        /// <remarks>Keys are case-insensitive. Query keys with prefixes such as '!' are considered distinct, separate query keys; the prefix should be included here.</remarks>
        public static Dictionary<string, ITileQueryFactory> TileQueryFactories = NativeTileQueryFactory.GetDefaultQueryFactories();

        /// <summary>Whether this instance's state has changed and it should be reinitialized next time it's used.</summary>
        private bool Dirty { get; set; } = false;

        /// <summary>A set of tile queries and their arguments, separated by commas.</summary>
        private string Condition { get; set; }

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; set; }

        /// <summary>A list of tile queries created by parsing <see cref="Condition"/>.</summary>
        private List<ITileQuery> Queries { get; set; }

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Chooses the query that should create the initial set of tiles, if any.</summary>
        /// <returns>The query that should create the initial set of tiles. Null if none of them can create it.</returns>
        public static ITileQuery ChooseStartingTilesSource(List<ITileQuery> queries)
        {
            ITileQuery highestQuery = null;
            foreach (var query in queries)
            {
                int thisPriority = query.StartingTilesPriority;
                if (thisPriority > int.MinValue) //if this query can create starting tiles
                {
                    if (highestQuery == null || highestQuery.StartingTilesPriority < thisPriority) //if no query has been chosen yet, or this one has higher priority
                        highestQuery = query; //select it as the highest priority so far
                }
            }
            return highestQuery;
        }

        /// <summary>Gets valid tiles from this query for the given location.</summary>
        /// <param name="randomizeOrder">If true, tiles will be returned in random order. If false, they'l be output in the order they're created, which depends on the queries used.</param>
        public IEnumerable<Vector2> GetTiles(bool randomizeOrder = true)
        {
            //handle null queries
            if (Queries == null)
            {
                //treat all tiles as valid
                foreach (var tile in GetAllTilesFromLocation(randomizeOrder))
                    yield return tile;
                yield break;
            }

            //reinitialize if needed
            if (Dirty)
                Initialize();
            Dirty = true;

            //create an initial tile set
            List<Vector2> tiles;
            if (ChooseStartingTilesSource(Queries) is ITileQuery startingQuery) //if a query should generate a starting list
            {
                tiles = startingQuery.GetStartingTiles();
                if (randomizeOrder)
                    FTMUtility.RandomizeList(tiles);
            }
            else //if the default starting list should be generated
                tiles = GetAllTilesFromLocation(true);

            //yield each valid tile
            foreach (Vector2 tile in tiles)
            {
                bool valid = true;
                foreach (var query in Queries)
                {
                    if (!query.CheckTile(tile))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                    yield return tile;
            }
        }

        /// <summary>Parses a comma-separated list of queries into functional handlers.</summary>
        /// <param name="location">The in-game location that these queries will check.</param>
        /// <param name="conditionString">A tile condition, i.e. a comma-separated list of query keys and their arguments.</param>
        /// <returns>A list of query handlers, sorted by <see cref="ITileQuery.CheckTilePriority"/>. Null if the condition string was blank.</returns>
        public static List<ITileQuery> ParseQueries(GameLocation location, string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString))
                return null;

            //split condition into queries by commas, but don't remove the quotes yet
            string[] rawQueries = ArgUtility.SplitQuoteAware(conditionString, ',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries, true);

            List<ITileQuery> queries = new(rawQueries.Length);

            foreach (string rawQuery in rawQueries)
            {
                string[] args = ArgUtility.SplitBySpaceQuoteAware(rawQuery); //split query into arguments around spaces (and remove empty entries)
                var query = TileQueryFactories[args[0]].CreateTileQuery(location, args); //note: this is intended to throw an exception if a key doesn't exist
                queries.Add(query);
            }

            queries.Sort((a, b) => b.CheckTilePriority.CompareTo(a.CheckTilePriority)); //sort by CheckTile priority from highest to lowest

            return queries;
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Creates a list of all tiles from this query's location.</summary>
        /// <param name="randomizeOrder">If true, the list will be in random order. If false, the list will be sorted in ascending order by height and width.</param>
        /// <returns>A list of all tiles from this query's location.</returns>
        private List<Vector2> GetAllTilesFromLocation(bool randomizeOrder = true)
        {
            int width = Location.map.Layers[0].LayerWidth;
            int height = Location.map.Layers[0].LayerHeight;

            List<Vector2> tiles = new(width * height);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles.Add(new Vector2(x, y));

            if (randomizeOrder)
                FTMUtility.RandomizeList(tiles);

            return tiles;
        }

        /// <summary>Creates this condition's queries or restores them to their initial states.</summary>
        private void Initialize()
        {
            Queries = ParseQueries(Location, Condition); //create or recreate all queries from the tile condition string
        }
    }
}