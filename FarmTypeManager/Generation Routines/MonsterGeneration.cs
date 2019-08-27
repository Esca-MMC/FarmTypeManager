using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class Generation
        {
            /// <summary>Generates monsters in the game based on the current player's config settings.</summary>
            public static void MonsterGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting monster generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting monster generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.MonsterSpawnEnabled)
                    {
                        Utility.Monitor.Log("Monster spawn is enabled. Starting generation process...", LogLevel.Trace);
                    }
                    else
                    {
                        Utility.Monitor.Log("Monster spawn is disabled.", LogLevel.Trace);
                        continue;
                    }

                    foreach (MonsterSpawnArea area in data.Config.Monster_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking monster settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        if (Game1.getLocationFromName(area.MapName) == null)
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. Monsters won't be spawned there.", LogLevel.Info);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"All extra conditions met. Validating list of monster types...", LogLevel.Trace);

                        //validate the provided monster types
                        List<MonsterType> validMonsterTypes = Utility.ValidateMonsterTypes(area.MonsterTypes, area.UniqueAreaID);

                        if (validMonsterTypes.Count <= 0)
                        {
                            Utility.Monitor.Log($"The monster type list is empty. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("Generating list of valid tiles...", LogLevel.Trace);

                        List<Vector2> validTiles = Utility.GenerateTileList(area, data.Save, data.Config.QuarryTileIndex, data.Config.Monster_Spawn_Settings.CustomTileIndex, false); //calculate a list of valid tiles for monsters in this area

                        Utility.Monitor.Log($"Number of valid tiles: {validTiles.Count}. Deciding how many items to spawn...", LogLevel.Trace);

                        //calculate how many monsters to spawn today
                        int spawnCount = Utility.RNG.Next(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay + 1); //random number from min to max

                        Utility.Monitor.Log($"Items to spawn: {spawnCount}. Beginning spawn process...", LogLevel.Trace);

                        //begin to spawn monsters
                        while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for monsters & still monsters to be spawned
                        {
                            //this section spawns 1 monster at a random valid location

                            spawnCount--; //reduce by 1, since one will be spawned

                            int randomIndex;
                            Vector2 randomTile;
                            bool tileConfirmed = false; //false until a valid monster location is confirmed
                            do
                            {
                                randomIndex = Utility.RNG.Next(validTiles.Count); //get the array index for a random valid tile
                                randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                                validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be invalidated now
                                tileConfirmed = Utility.IsTileValid(area, randomTile, false); //check whether the tile is open/valid for spawning
                            } while (validTiles.Count > 0 && !tileConfirmed);

                            if (!tileConfirmed) { break; } //if no more valid tiles could be found, stop trying to spawn things in this area

                            MonsterType randomMonster = area.MonsterTypes[Utility.RNG.Next(area.MonsterTypes.Count)]; //get a random monster type to spawn

                            Utility.SpawnMonster(randomMonster, Game1.getLocationFromName(area.MapName), randomTile, area.UniqueAreaID);

                            //TODO: fix the savedobject system
                            /*
                            if (area.DaysUntilSpawnsExpire != null) //if this area assigns expiration dates to objects
                            {
                                SavedObject saved = new SavedObject(area.MapName, randomTile, SavedObject.ObjectType.LargeObject, randomMonster, null, area.DaysUntilSpawnsExpire); //create a record of the newly spawned object
                                data.Save.SavedObjects.Add(saved); //add it to the save file with the area's expiration setting
                            }
                            */
                        }

                        Utility.Monitor.Log($"Monster spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Monster spawn complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Monster spawn complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Monster spawn process complete.", LogLevel.Trace);
            }
        }
    }
}
