using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Determines whether a specific tile on a map is valid for object placement.</summary>
            /// <param name="location">The game location to check.</param>
            /// <param name="tile">The tile to validate for object placement. If the object is larger than 1 tile, this is the top left corner.</param>
            /// <param name="size">A point representing the object's size in tiles (horizontal, vertical).</param>
            /// <param name="strictTileChecking">The level of strictness to use when checking tiles: "None", "Low", "Medium, "High", or "Maximum". Unknown values default to "Maximum".</param>
            /// <returns>True if the tile is currently valid for object placement.</returns>
            public static bool IsTileValid(GameLocation location, Vector2 tile, Point size, string strictTileChecking)
            {
                if (strictTileChecking.Equals("none", StringComparison.OrdinalIgnoreCase)) //if no validation is needed
                    return true;

                //quicker 1x1 check
                if (size.X == 1 && size.Y == 1)
                    return IsSingleTileValid(location, tile, strictTileChecking);

                //multi-tile check
                for (int x = 0; x < size.X; x++)
                    for (int y = 0; y < size.Y; y++)
                        if (!IsSingleTileValid(location, new Vector2(tile.X + x, tile.Y + y), strictTileChecking)) //if any tile in the specified area is invalid
                            return false;

                return true;
            }

            /// <summary>Determines whether a single tile is valid for object placement at the given strictness level.</summary>
            /// <param name="location">The game location to check.</param>
            /// <param name="tile">The tile to validate.</param>
            /// <param name="strictTileChecking">The level of strictness to use when checking tiles: "None", "Low", "Medium, "High", or "Maximum". Unknown values default to "Maximum".</param>
            /// <returns>True if the tile is currently valid for object placement.</returns>
            /// <remarks>This method is a component of <see cref="IsTileValid"/>. Other code should call that method instead.</remarks>
            private static bool IsSingleTileValid(GameLocation location, Vector2 tile, string strictTileChecking)
            {
                if (strictTileChecking.Equals("low", StringComparison.OrdinalIgnoreCase))
                    return !location.isObjectAtTile((int)tile.X, (int)tile.Y); //false if this tile already contains an object

                else if (strictTileChecking.Equals("medium", StringComparison.OrdinalIgnoreCase))
                    return !location.IsTileOccupiedBy(tile); //false if this tile is occupied

                else if (strictTileChecking.Equals("high", StringComparison.OrdinalIgnoreCase))
                    return !location.IsTileOccupiedBy(tile) && location.CanItemBePlacedHere(tile); //false if this tile is occupied or obstructed in any way

                //maximum strictness (default)
                return !location.IsNoSpawnTile(tile) && !location.IsTileOccupiedBy(tile) && location.CanItemBePlacedHere(tile); //false if this tile has "NoSpawn", is occupied, or is obstructed in any way
            }
        }
    }
}