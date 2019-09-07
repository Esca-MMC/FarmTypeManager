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
            /// <summary>Generates forageable items in the game based on the current player's config settings.</summary>
            public static void ForageGeneration()
            {
                foreach (FarmData data in Utility.FarmDataList)
                {
                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"Starting forage generation for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"Starting forage generation for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }

                    if (data.Config.ForageSpawnEnabled)
                    {
                        Utility.Monitor.Log("Forage spawn is enabled. Starting generation process...", LogLevel.Trace);
                    }
                    else
                    {
                        Utility.Monitor.Log("Forage spawn is disabled for this file.", LogLevel.Trace);
                        continue;
                    }

                    foreach (ForageSpawnArea area in data.Config.Forage_Spawn_Settings.Areas)
                    {
                        Utility.Monitor.Log($"Checking forage settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);

                        //validate the map name for the area
                        if (Game1.getLocationFromName(area.MapName) == null)
                        {
                            Utility.Monitor.Log($"No map named \"{area.MapName}\" could be found. No forage will be spawned there.", LogLevel.Info);
                            continue;
                        }

                        //validate extra conditions, if any
                        if (Utility.CheckExtraConditions(area, data.Save) != true)
                        {
                            Utility.Monitor.Log($"Extra conditions prevent spawning in this area. Next area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log("All extra conditions met. Generating list of valid tiles...", LogLevel.Trace);

                        List<Vector2> validTiles = Utility.GenerateTileList(area, data.Save, data.Config.QuarryTileIndex, data.Config.Forage_Spawn_Settings.CustomTileIndex, false); //calculate a list of valid tiles for forage in this area

                        Utility.Monitor.Log($"Number of valid tiles: {validTiles.Count}. Deciding how many items to spawn...", LogLevel.Trace);

                        //calculate how much forage to spawn today
                        int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, data.Config.Forage_Spawn_Settings.PercentExtraSpawnsPerForagingLevel, Utility.Skills.Foraging);

                        Utility.Monitor.Log($"Items to spawn: {spawnCount}. Retrieving list of forage types...", LogLevel.Trace);

                        object[] forageObjects = null; //the unprocessed array of forage types to use for this area today

                        switch (Game1.currentSeason)
                        {
                            case "spring":
                                if (area.SpringItemIndex != null) //if there's an "override" list set for this area
                                {
                                    if (area.SpringItemIndex.Length > 0) //if the override includes any items
                                    {
                                        forageObjects = area.SpringItemIndex; //get the override index list for this area
                                    }
                                    //if an area index exists but is empty, *do not* use the main index; users may want to disable spawns in this season
                                }
                                else if (data.Config.Forage_Spawn_Settings.SpringItemIndex.Length > 0) //if no "override" list exists and the main index list includes any items
                                {
                                    forageObjects = data.Config.Forage_Spawn_Settings.SpringItemIndex; //get the main index list
                                }
                                break;
                            case "summer":
                                if (area.SummerItemIndex != null)
                                {
                                    if (area.SummerItemIndex.Length > 0)
                                    {
                                        forageObjects = area.SummerItemIndex;
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.SummerItemIndex.Length > 0)
                                {
                                    forageObjects = data.Config.Forage_Spawn_Settings.SummerItemIndex;
                                }
                                break;
                            case "fall":
                                if (area.FallItemIndex != null)
                                {
                                    if (area.FallItemIndex.Length > 0)
                                    {
                                        forageObjects = area.FallItemIndex;
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.FallItemIndex.Length > 0)
                                {
                                    forageObjects = data.Config.Forage_Spawn_Settings.FallItemIndex;
                                }
                                break;
                            case "winter":
                                if (area.WinterItemIndex != null)
                                {
                                    if (area.WinterItemIndex.Length > 0)
                                    {
                                        forageObjects = area.WinterItemIndex;
                                    }
                                }
                                else if (data.Config.Forage_Spawn_Settings.WinterItemIndex.Length > 0)
                                {
                                    forageObjects = data.Config.Forage_Spawn_Settings.WinterItemIndex;
                                }
                                break;
                        }

                        if (forageObjects == null) //no valid forage list was selected
                        {
                            Utility.Monitor.Log($"No forage list selected. This generally means the {Game1.currentSeason}IndexList was empty. Skipping to the next forage area...", LogLevel.Trace);
                            continue;
                        }

                        //a list was selected, so parse "forageObjects" into a list of valid forage IDs
                        List<int> forageIDs = Utility.GetIDsFromObjects(forageObjects.ToList(), area.UniqueAreaID);

                        if (forageIDs.Count <= 0) //no valid items were added to the list
                        {
                            Utility.Monitor.Log($"Forage list selected, but contained no valid forage items. Skipping to the next forage area...", LogLevel.Trace);
                            continue;
                        }

                        Utility.Monitor.Log($"Forage types found: {forageIDs.Count}. Beginning spawn process...", LogLevel.Trace);

                        //begin to spawn forage; each loop should spawn 1 random forage object on a random valid tile
                        while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for forage & still forage to be spawned
                        {
                            spawnCount--; //reduce by 1, since one will be spawned
                            int randomIndex = Utility.RNG.Next(validTiles.Count); //get the array index for a random valid tile
                            Vector2 randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                            validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed now

                            int randomForage = forageIDs[Utility.RNG.Next(forageIDs.Count)]; //pick a random forage ID from the list

                            Utility.SpawnForage(randomForage, Game1.getLocationFromName(area.MapName), randomTile);
                            
                            if (area.DaysUntilSpawnsExpire.HasValue) //if this area assigns expiration dates to forage
                            {
                                SavedObject saved = new SavedObject(area.MapName, randomTile, SavedObject.ObjectType.Forage, randomForage, null, area.DaysUntilSpawnsExpire.Value); //create a record of the newly spawned forage
                                data.Save.SavedObjects.Add(saved); //add it to the save file with the area's expiration setting
                            }
                        }

                        Utility.Monitor.Log($"Forage spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Trace);
                    }

                    if (data.Pack != null) //content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Forage spawn complete for this content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                    }
                    else //not a content pack
                    {
                        Utility.Monitor.Log($"All areas checked. Forage spawn complete for this file: FarmTypeManager/data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                    }
                }

                Utility.Monitor.Log("All files and content packs checked. Forage spawn process complete.", LogLevel.Trace);
            }
        }
    }
}
