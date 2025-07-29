using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A factory that generates instances of this mod's built-in tile query types.</summary>
    public class NativeTileQueryFactory : ITileQueryFactory
    {
        /**************/
        /* ITileQuery */
        /**************/
        public ITileQuery CreateTileQuery(GameLocation location, string[] queryArgs)
        {
            if (queryArgs == null || queryArgs.Length == 0 || string.IsNullOrWhiteSpace(queryArgs[0]))
                throw new ArgumentException("Query is null, empty, or blank.", nameof(queryArgs));

            switch (queryArgs[0].ToUpperInvariant())
            {
                //basic
                case "FALSE":
                case "!TRUE":
                    return new FalseTileQuery();
                case "TRUE":
                case "!FALSE":
                    return new TrueTileQuery();
                case "EXACT":
                    return new ExactTileQuery(location, queryArgs);
                case "!EXACT":
                    return new NotExactTileQuery(queryArgs);

                //meta
                case "ANY":
                    return new AnyTileQuery(location, queryArgs);
                case "!ANY":
                    return new NotAnyTileQuery(location, queryArgs);
                case "NOT":
                    return new NotTileQuery(location, queryArgs);
                case "SIZE":
                    return new SizeTileQuery(location, queryArgs);

                //ranges
                case "AREA_WH":
                    return new AreaWHTileQuery(location, queryArgs);
                case "!AREA_WH":
                    return new NotAreaWHTileQuery(queryArgs);
                case "AREA_XY":
                    return new AreaXYTileQuery(location, queryArgs);
                case "!AREA_XY":
                    return new NotAreaXYTileQuery(queryArgs);

                //simple properties
                case "INDEX":
                    return new IndexTileQuery(location, queryArgs);
                case "!INDEX":
                    return new NotIndexTileQuery(location, queryArgs);
                case "PROPERTY":
                    return new PropertyTileQuery(location, queryArgs);
                case "!PROPERTY":
                    return new NotPropertyTileQuery(location, queryArgs);

                //complex properties
                case "PASSABLE":
                    return new PassableTileQuery(location);
                case "!PASSABLE":
                    return new NotPassableTileQuery(location);
                case "HAS_OBJECT":
                    return new HasObjectTileQuery(location);
                case "!HAS_OBJECT":
                    return new NotHasObjectTileQuery(location);
                case "OCCUPIED":
                    return new OccupiedTileQuery(location);
                case "!OCCUPIED":
                    return new NotOccupiedTileQuery(location);
                case "CAN_PLACE_ITEM":
                    return new CanPlaceItemTileQuery(location);
                case "!CAN_PLACE_ITEM":
                    return new CannotPlaceItemTileQuery(location);

                //unknown properties (e.g. sent to this handler incorrectly)
                default:
                    throw new ArgumentException($"Query key '{queryArgs[0]}' is not recognized by this factory type.");
            }
        }

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Creates a case-insensitive dictionary populated with all built-in query keys, each associated with a shared instance of this class.</summary>
        public static Dictionary<string, ITileQueryFactory> GetDefaultQueryFactories()
        {
            var factory = new NativeTileQueryFactory();

            Dictionary<string, ITileQueryFactory> factories = new(StringComparer.OrdinalIgnoreCase)
            {
                { "FALSE", factory },
                { "!FALSE", factory },
                { "TRUE", factory },
                { "!TRUE", factory },
                { "EXACT", factory },
                { "!EXACT", factory },


                { "ANY", factory },
                { "!ANY", factory },
                { "NOT", factory },
                { "SIZE", factory },

                { "AREA_WH", factory },
                { "!AREA_WH", factory },
                { "AREA_XY", factory },
                { "!AREA_XY", factory },

                { "INDEX", factory },
                { "!INDEX", factory },
                { "PROPERTY", factory },
                { "!PROPERTY", factory },

                { "CAN_PLACE_ITEM", factory },
                { "!CAN_PLACE_ITEM", factory },
                { "HAS_OBJECT", factory },
                { "!HAS_OBJECT", factory },
                { "OCCUPIED", factory },
                { "!OCCUPIED", factory },
                { "PASSABLE", factory },
                { "!PASSABLE", factory },
            };

            return factories;
        }
    }
}