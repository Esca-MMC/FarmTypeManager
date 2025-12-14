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
                //meta
                case "FALSE":
                case "!TRUE":
                    return new FalseTileQuery();
                case "TRUE":
                case "!FALSE":
                    return new TrueTileQuery();
                case "ANY":
                    return new AnyTileQuery(location, queryArgs);
                case "!ANY":
                    return new NotAnyTileQuery(location, queryArgs);
                case "NOT":
                    return new NotTileQuery(location, queryArgs);
                case "SIZE":
                    return new SizeTileQuery(location, queryArgs);
                case "!SIZE":
                    return new NotSizeTileQuery(location, queryArgs);
                case "SIZE_MAP":
                    return new SizeMapTileQuery(location, queryArgs);
                case "!SIZE_MAP":
                    return new NotSizeMapTileQuery(location, queryArgs);

                //ranges
                case "AREA_WH":
                    return new AreaWHTileQuery(location, queryArgs);
                case "!AREA_WH":
                    return new NotAreaWHTileQuery(queryArgs);
                case "AREA_XY":
                    return new AreaXYTileQuery(location, queryArgs);
                case "!AREA_XY":
                    return new NotAreaXYTileQuery(queryArgs);
                case "AREA_CIRCLE":
                    return new AreaCircleTileQuery(location, queryArgs);
                case "!AREA_CIRCLE":
                    return new NotAreaCircleTileQuery(queryArgs);
                case "AREA_DIAMOND":
                    return new AreaDiamondTileQuery(location, queryArgs);
                case "!AREA_DIAMOND":
                    return new NotAreaDiamondTileQuery(queryArgs);
                case "AREA_MAP":
                    return new AreaMapTileQuery(location, queryArgs);
                case "!AREA_MAP":
                    return new NotAreaMapTileQuery(queryArgs);

                //simple properties
                case "EXACT":
                    return new ExactTileQuery(location, queryArgs);
                case "!EXACT":
                    return new NotExactTileQuery(queryArgs);
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
                case "OCCUPIED":
                    return new OccupiedTileQuery(location);
                case "!OCCUPIED":
                    return new NotOccupiedTileQuery(location);
                case "CAN_PLACE_ITEM":
                    return new CanPlaceItemTileQuery(location);
                case "!CAN_PLACE_ITEM":
                    return new NotCanPlaceItemTileQuery(location);

                //objects
                case "HAS_OBJECT":
                    return new HasObjectTileQuery(location);
                case "!HAS_OBJECT":
                    return new NotHasObjectTileQuery(location);
                case "HAS_OBJECT_ID":
                    return new HasObjectIDTileQuery(location, queryArgs);
                case "!HAS_OBJECT_ID":
                    return new NotHasObjectIDTileQuery(location, queryArgs);
                case "HAS_FURNITURE":
                    return new HasFurnitureTileQuery(location);
                case "!HAS_FURNITURE":
                    return new NotHasFurnitureTileQuery(location);
                case "HAS_FURNITURE_ID":
                    return new HasFurnitureIdTileQuery(location, queryArgs);
                case "!HAS_FURNITURE_ID":
                    return new NotHasFurnitureIdTileQuery(location, queryArgs);
                case "HAS_SMALL_TERRAIN_FEATURE":
                    return new HasSmallTerrainFeatureTileQuery(location);
                case "!HAS_SMALL_TERRAIN_FEATURE":
                    return new NotHasSmallTerrainFeatureTileQuery(location);
                case "HAS_LARGE_TERRAIN_FEATURE":
                    return new HasLargeTerrainFeatureTileQuery(location);
                case "!HAS_LARGE_TERRAIN_FEATURE":
                    return new NotHasLargeTerrainFeatureTileQuery(location);
                case "HAS_RESOURCE_CLUMP":
                    return new HasResourceClumpTileQuery(location);
                case "!HAS_RESOURCE_CLUMP":
                    return new NotHasResourceClumpTileQuery(location);

                //characters
                case "HAS_CHARACTER":
                    return new HasCharacterTileQuery(location);
                case "!HAS_CHARACTER":
                    return new NotHasCharacterTileQuery(location);

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
                //meta
                { "FALSE", factory },
                { "!FALSE", factory },
                { "TRUE", factory },
                { "!TRUE", factory },
                { "ANY", factory },
                { "!ANY", factory },
                { "NOT", factory },
                { "SIZE", factory },
                { "!SIZE", factory },
                { "SIZE_MAP", factory },
                { "!SIZE_MAP", factory },

                //ranges
                { "AREA_WH", factory },
                { "!AREA_WH", factory },
                { "AREA_XY", factory },
                { "!AREA_XY", factory },
                { "AREA_CIRCLE", factory },
                { "!AREA_CIRCLE", factory },
                { "AREA_DIAMOND", factory },
                { "!AREA_DIAMOND", factory },
                { "AREA_MAP", factory },
                { "!AREA_MAP", factory },

                //simple properties
                { "EXACT", factory },
                { "!EXACT", factory },
                { "INDEX", factory },
                { "!INDEX", factory },
                { "PROPERTY", factory },
                { "!PROPERTY", factory },

                //complex properties
                { "PASSABLE", factory },
                { "!PASSABLE", factory },
                { "OCCUPIED", factory },
                { "!OCCUPIED", factory },
                { "CAN_PLACE_ITEM", factory },
                { "!CAN_PLACE_ITEM", factory },
                
                //objects
                { "HAS_OBJECT", factory },
                { "!HAS_OBJECT", factory },
                { "HAS_OBJECT_ID", factory },
                { "!HAS_OBJECT_ID", factory },
                { "HAS_FURNITURE", factory },
                { "!HAS_FURNITURE", factory },
                { "HAS_FURNITURE_ID", factory },
                { "!HAS_FURNITURE_ID", factory },
                { "HAS_SMALL_TERRAIN_FEATURE", factory },
                { "!HAS_SMALL_TERRAIN_FEATURE", factory },
                { "HAS_LARGE_TERRAIN_FEATURE", factory },
                { "!HAS_LARGE_TERRAIN_FEATURE", factory },
                { "HAS_RESOURCE_CLUMP", factory },
                { "!HAS_RESOURCE_CLUMP", factory },

                //characters
                {"HAS_CHARACTER", factory },
                {"!HAS_CHARACTER", factory }
            }
            ;

            return factories;
        }
    }
}