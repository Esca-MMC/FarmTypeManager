using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Determines whether a specific tile on a map is valid for object placement, using any necessary checks from Stardew's native methods.</summary>
            /// <param name="location">The game location to be checked.</param>
            /// <param name="tile">The tile to be validated for object placement (for a large object, this is effectively its upper left corner).</param>
            /// <param name="size">A point representing the size of this object in tiles.</param>
            /// <returns>Whether the provided tile is valid for the given area and object size, based on the area's StrictTileChecking setting.</returns>
            public static bool IsTileValid(GameLocation location, Vector2 tile, Point size, string strictTileChecking)
            {
                bool valid = true; //whether the provided tile is valid with the given parameters

                List<Vector2> tilesToCheck = new List<Vector2>(); //a list of tiles that need to be valid (based on spawn object size)

                for (int x = 0; x < size.X; x++)
                {
                    for (int y = 0; y < size.Y; y++)
                    {
                        tilesToCheck.Add(new Vector2(tile.X + x, tile.Y + y));
                    }
                }

                if (strictTileChecking.Equals("none", StringComparison.OrdinalIgnoreCase)) //no validation at all
                {
                    valid = true;
                }
                else if (strictTileChecking.Equals("low", StringComparison.OrdinalIgnoreCase)) //low-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.isObjectAtTile((int)t.X, (int)t.Y)) //if this tile is blocked by another object
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }
                else if (strictTileChecking.Equals("medium", StringComparison.OrdinalIgnoreCase)) //medium-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.IsTileOccupiedBy(t)) //if this tile is occupied
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }
                else if (strictTileChecking.Equals("high", StringComparison.OrdinalIgnoreCase)) //high-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (!location.CanItemBePlacedHere(t)) //if the tile is *not* totally clear
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }
                else //max-strictness validation
                {
                    foreach (Vector2 t in tilesToCheck) //for each tile to be checked
                    {
                        if (location.IsNoSpawnTile(t) || !location.CanItemBePlacedHere(t)) //if this tile has "NoSpawn", is *not* totally clear
                        {
                            valid = false; //prevent spawning here
                            break; //skip checking the other tiles
                        }
                    }
                }

                return valid;
            }
        }
    }
}