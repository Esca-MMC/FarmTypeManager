using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A factory that generates instances of this mod's built-in tile query types.</summary>
    public class BasicTileQueryFactory : ITileQueryFactory
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

                /*case "SIZE":
                    return ???;
                case "!SIZE":
                    return ???;
                case "ANY":
                    return ???;
                case "!ANY":
                    return ???;*/

                case "AREA_WH":
                    return new AreaWHTileQuery(queryArgs);
                case "!AREA_WH":
                    return new NotAreaWHTileQuery(queryArgs);
                case "AREA_XY":
                    return new AreaXYTileQuery(queryArgs);
                case "!AREA_XY":
                    return new NotAreaXYTileQuery(queryArgs);
                /*
            case "PROPERTY":
                return ???;
            case "!PROPERTY":
                return ???;
            case "INDEX":
                return ???;
            case "!INDEX":
                return ???;

            case "PASSABLE":
                return ???;
            case "!PASSABLE":
                return ???;
            case "ALLOWS_OBJECTS":
                return ???;
            case "!ALLOWS_OBJECTS":
                return ???;
            case "OCCUPIED":
                return ???;
            case "!OCCUPIED":
                return ???;*/
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
            var factory = new BasicTileQueryFactory();

            Dictionary<string, ITileQueryFactory> factories = new(StringComparer.OrdinalIgnoreCase)
            {
                { "TRUE", factory },
                { "!TRUE", factory },
                { "FALSE", factory },
                { "!FALSE", factory },

                { "SIZE", factory },
                { "!SIZE", factory },
                { "ANY", factory },
                { "!ANY", factory },

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
                { "ALLOWS_OBJECTS", factory },
                { "!ALLOWS_OBJECTS", factory },
                { "OCCUPIED", factory },
                { "!OCCUPIED", factory },
                { "CAN_PLACE_ITEM", factory },
                { "!CAN_PLACE_ITEM", factory },
            };

            return factories;
        }
    }
}