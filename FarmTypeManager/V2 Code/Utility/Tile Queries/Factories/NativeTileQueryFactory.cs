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
                case "TRUE":
                case "!FALSE":
                    return new TrueTileQuery();
                case "FALSE":
                case "!TRUE":
                    return new FalseTileQuery();

                case "SIZE":
                    return new SizeTileQuery(location, queryArgs);
                /*case "ANY":
                    return ???;
                case "!ANY":
                    return ???;*/
                case "NOT":
                    return new NotTileQuery(location, queryArgs);

                case "AREA_WH":
                    return new AreaWHTileQuery(queryArgs);
                case "!AREA_WH":
                    return new NotAreaWHTileQuery(queryArgs);
                case "AREA_XY":
                    return new AreaXYTileQuery(queryArgs);
                case "!AREA_XY":
                    return new NotAreaXYTileQuery(queryArgs);

                case "PROPERTY":
                    return new PropertyTileQuery(location, queryArgs);
                case "!PROPERTY":
                    return new NotPropertyTileQuery(location, queryArgs);
                case "INDEX":
                    return new IndexTileQuery(location, queryArgs);
                case "!INDEX":
                    return new NotIndexTileQuery(location, queryArgs);

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
                { "TRUE", factory },
                { "!TRUE", factory },
                { "FALSE", factory },
                { "!FALSE", factory },

                { "SIZE", factory },
                { "ANY", factory },
                { "!ANY", factory },
                { "NOT", factory },

                { "AREA_WH", factory },
                { "!AREA_WH", factory },
                { "AREA_XY", factory },
                { "!AREA_XY", factory },

                { "PROPERTY", factory },
                { "!PROPERTY", factory },
                { "INDEX", factory },
                { "!INDEX", factory },

                { "PASSABLE", factory },
                { "!PASSABLE", factory },
                { "HAS_OBJECT", factory },
                { "!HAS_OBJECT", factory },
                { "OCCUPIED", factory },
                { "!OCCUPIED", factory },
                { "CAN_PLACE_ITEM", factory },
                { "!CAN_PLACE_ITEM", factory },
            };

            return factories;
        }
    }
}