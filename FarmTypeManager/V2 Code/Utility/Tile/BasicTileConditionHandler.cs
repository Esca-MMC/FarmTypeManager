using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager
{
    /// <summary>Handles this mod's built-in tile conditions for the tile query system.</summary>
    public class BasicTileConditionHandler : ITileConditionHandler
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>A case-insensitive set of handled condition keys and their <see cref="EvalPriority"/> levels.</summary>
        public IDictionary<string, int> ConditionPriorities { get; } = new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase)
        {
            { "TRUE", ITileConditionHandler.EvalPriority_High },
            { "!TRUE", ITileConditionHandler.EvalPriority_High },
            { "FALSE", ITileConditionHandler.EvalPriority_High },
            { "!FALSE", ITileConditionHandler.EvalPriority_High },

            { "AREA_WH", ITileConditionHandler.EvalPriority_High },
            { "!AREA_WH", ITileConditionHandler.EvalPriority_High },
            { "AREA_XY", ITileConditionHandler.EvalPriority_High },
            { "!AREA_XY", ITileConditionHandler.EvalPriority_High },

            { "PROPERTY", ITileConditionHandler.EvalPriority_Normal },
            { "!PROPERTY", ITileConditionHandler.EvalPriority_Normal },
            { "INDEX", ITileConditionHandler.EvalPriority_Normal },
            { "!INDEX", ITileConditionHandler.EvalPriority_Normal },
        };

        /******************/
        /* Public methods */
        /******************/

        /// <inheritdoc/>
        public IEnumerable<Vector2> FilterTiles(string[] args, GameLocation location, IEnumerable<Vector2> tiles)
        {
            IEnumerable<Vector2> filter;

            //select the handler method based on the condition key (upper case)
            switch (args[0].ToUpperInvariant())
            {
                case "TRUE":
                    filter = Impl_TRUE(tiles);
                    break;
                case "!TRUE":
                    yield break; //return nothing
                case "FALSE":
                    yield break;
                case "!FALSE":
                    filter = Impl_TRUE(tiles);
                    break;

                case "AREA_WH":
                    filter = Impl_AREA_WH(args, tiles);
                    break;
                case "!AREA_WH":
                    filter = Impl_NOT_AREA_WH(args, tiles);
                    break;
                case "AREA_XY":
                    filter = Impl_AREA_XY(args, tiles);
                    break;
                case "!AREA_XY":
                    filter = Impl_NOT_AREA_XY(args, tiles);
                    break;

                case "PROPERTY":
                    filter = Impl_PROPERTY(args, location, tiles);
                    break;
                case "!PROPERTY":
                    filter = Impl_NOT_PROPERTY(args, location, tiles);
                    break;
                case "INDEX":
                    filter = Impl_INDEX(args, location, tiles);
                    break;
                case "!INDEX":
                    filter = Impl_NOT_INDEX(args, location, tiles);
                    break;

                default:
                    yield break;
            }

            foreach (Vector2 validTile in filter) //lazily iterate over each filtered tile
                yield return validTile;
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Returns all tiles.</summary>
        private static IEnumerable<Vector2> Impl_TRUE(IEnumerable<Vector2> tiles)
        {
            foreach (Vector2 tile in tiles)
                yield return tile;
        }

        /// <summary>Returns tiles within a specified area (x, y, width, height).</summary>
        private static IEnumerable<Vector2> Impl_AREA_WH(string[] args, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGetRectangle(args, 1, out Rectangle rectangle, out string error, $"Area Rectangle"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            foreach (Vector2 tile in tiles)
            {
                if (rectangle.Contains(tile))
                    yield return tile;
            }
        }

        /// <summary>Returns tiles that are NOT within a specified area (x, y, width, height).</summary>
        private static IEnumerable<Vector2> Impl_NOT_AREA_WH(string[] args, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGetRectangle(args, 1, out Rectangle rectangle, out string error, $"Area Rectangle"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            foreach (Vector2 tile in tiles)
            {
                if (!rectangle.Contains(tile))
                    yield return tile;
            }
        }

        /// <summary>Returns tiles within a specified area (top left x and y, bottom left x and y, inclusively).</summary>
        private static IEnumerable<Vector2> Impl_AREA_XY(string[] args, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGetVector2(args, 1, out Vector2 topLeftCorner, out string error, integerOnly: true, name: $"Area XY (Top Left Corner)") ||
                !ArgUtility.TryGetVector2(args, 3, out Vector2 bottomRightCorner, out error, integerOnly: true, name: $"Area XY (Bottom Right Corner)"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            foreach (Vector2 tile in tiles)
            {
                if (tile.X >= topLeftCorner.X &&
                    tile.X <= bottomRightCorner.X &&
                    tile.Y >= topLeftCorner.Y &&
                    tile.Y <= bottomRightCorner.Y)
                {
                    yield return tile;
                }
            }
        }

        /// <summary>Returns tiles that are NOT within a specified area (top left x and y, bottom left x and y, inclusively).</summary>
        private static IEnumerable<Vector2> Impl_NOT_AREA_XY(string[] args, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGetVector2(args, 1, out Vector2 topLeftCorner, out string error, integerOnly: true, name: $"Area XY (Top Left Corner)") ||
                !ArgUtility.TryGetVector2(args, 3, out Vector2 bottomRightCorner, out error, integerOnly: true, name: $"Area XY (Bottom Right Corner)"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            foreach (Vector2 tile in tiles)
            {
                if ((tile.X >= topLeftCorner.X &&
                    tile.X <= bottomRightCorner.X &&
                    tile.Y >= topLeftCorner.Y &&
                    tile.Y <= bottomRightCorner.Y)
                    == false)
                {
                    yield return tile;
                }
            }
        }

        /// <summary>Returns tiles with the specified map property and optional value(s).</summary>
        private static IEnumerable<Vector2> Impl_PROPERTY(string[] args, GameLocation location, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGet(args, 1, out string layerName, out string error, false, "Map Layer Name") ||
                !ArgUtility.TryGet(args, 2, out string key, out error, false, "Tile Property Key"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            foreach (Vector2 tile in tiles)
            {
                string value = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, key, layerName, ignoreTileSheetProperties: false); //get the tile property's current value

                if (value != null)
                {
                    if (args.Length > 3) //if optional comparison values were provided
                    {
                        for (int x = 3; x < args.Length; x++)
                        {
                            if (string.Equals(value, args[x], StringComparison.OrdinalIgnoreCase)) //if the value matches this argument
                            {
                                yield return tile;
                                break;
                            }
                        }
                    }
                    else
                        yield return tile;
                }
            }
        }

        /// <summary>Returns tiles that do NOT have the specified map property and optional value(s).</summary>
        private static IEnumerable<Vector2> Impl_NOT_PROPERTY(string[] args, GameLocation location, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGet(args, 1, out string layerName, out string error, false, "Map Layer Name") ||
                !ArgUtility.TryGet(args, 2, out string key, out error, false, "Tile Property Key"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            foreach (Vector2 tile in tiles)
            {
                string value = location.doesTileHaveProperty((int)tile.X, (int)tile.Y, key, layerName, ignoreTileSheetProperties: false); //get the tile property's current value

                if (value == null)
                    yield return tile;
                else
                {
                    if (args.Length > 3) //if optional comparison values were provided
                    {
                        bool matches = false;
                        for (int x = 3; x < args.Length; x++)
                        {
                            if (string.Equals(value, args[x], StringComparison.OrdinalIgnoreCase)) //if the value matches this argument
                            {
                                matches = true;
                                break;
                            }
                        }
                        if (!matches) //if it does NOT match any comparison values
                            yield return tile;
                    }
                }
            }
        }

        /// <summary>Returns tiles that match the specified tilesheet indices. If none are provided, returns tiles with any index.</summary>
        private static IEnumerable<Vector2> Impl_INDEX(string[] args, GameLocation location, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGet(args, 1, out string layerName, out string error, false, "Map Layer Name"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            HashSet<int> comparisonIndices = new(args.Length - 2);

            for (int x = 2; x < args.Length; x++) //for each optional comparison value
            {
                if (!ArgUtility.TryGetInt(args, x, out int comparisonIndex, out error, $"Tilesheet Index"))
                {
                    FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                    yield break;
                }
                comparisonIndices.Add(comparisonIndex);
            }

            foreach (Vector2 tile in tiles)
            {
                int tileIndex = location.getTileIndexAt((int)tile.X, (int)tile.Y, layerName, tilesheetId: null); //get the tile's index from the specified layer (-1 if not found)

                if (tileIndex != -1)
                {
                    if (comparisonIndices.Count > 0) //if optional comparison values were provided
                    {
                        if (comparisonIndices.Contains(tileIndex)) //if any comparison index matches the tile's index
                            yield return tile;
                    }
                    else
                        yield return tile;
                }
            }
        }

        //// <summary>Returns tiles that do NOT match the specified tilesheet indices. If none are provided, returns tiles that do NOT have an index.</summary>
        private static IEnumerable<Vector2> Impl_NOT_INDEX(string[] args, GameLocation location, IEnumerable<Vector2> tiles)
        {
            if (!ArgUtility.TryGet(args, 1, out string layerName, out string error, false, "Map Layer Name"))
            {
                FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                yield break;
            }

            HashSet<int> comparisonIndices = new(args.Length - 2);

            for (int x = 2; x < args.Length; x++) //for each optional comparison value
            {
                if (!ArgUtility.TryGetInt(args, x, out int comparisonIndex, out error, $"Tilesheet Index"))
                {
                    FTMUtility.Monitor.LogOnce($"Invalid tile condition: \"{string.Join(' ', args)}\". Reason: \"{error}\".", LogLevel.Warn);
                    yield break;
                }
                comparisonIndices.Add(comparisonIndex);
            }

            foreach (Vector2 tile in tiles)
            {
                int tileIndex = location.getTileIndexAt((int)tile.X, (int)tile.Y, layerName, tilesheetId: null); //get the tile's index from the specified layer (-1 if not found)

                if (tileIndex == -1) //if this tile has no index
                    yield return tile;
                else
                {
                    if (comparisonIndices.Count > 0) //if optional comparison values were provided
                    {
                        if (!comparisonIndices.Contains(tileIndex)) //if NO comparison index matches the tile's index
                            yield return tile;
                    }
                }
            }
        }
    }
}