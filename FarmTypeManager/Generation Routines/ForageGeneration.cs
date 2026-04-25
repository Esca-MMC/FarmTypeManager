using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

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
                try
                {
                    foreach (FarmData data in Utility.FarmDataList)
                    {
                        if (data.Config?.ForageSpawnEnabled == true && data.Config.Forage_Spawn_Settings != null)
                        {
                            if (data.Pack != null)
                                Utility.Monitor.Log($"Generating forage for content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                            else
                                Utility.Monitor.Log($"Generating forage for local file: data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                        }
                        else
                        {
                            if (data.Pack != null)
                                Utility.Monitor.VerboseLog($"Forage generation is disabled for content pack: {data.Pack.Manifest.Name}");
                            else
                                Utility.Monitor.VerboseLog($"Forage generation is disabled for local file: data/{Constants.SaveFolderName}.json");

                            continue;
                        }

                        foreach (ForageSpawnArea area in data.Config.Forage_Spawn_Settings.Areas)
                        {
                            Utility.Monitor.VerboseLog($"Checking forage settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})");

                            //validate the map name for the area
                            List<string> locations = Utilities.Locations.GetLocationNames(area.MapName); //get all locations for this map name
                            if (locations.Count == 0) //if no locations were found
                            {
                                Utility.Monitor.VerboseLog($"No map named \"{area.MapName}\" could be found. Skipping this area.");
                                continue;
                            }

                            //validate extra conditions, if any
                            if (Utility.CheckExtraConditions(area, data.Save, data.Pack?.Manifest) != true)
                            {
                                Utility.Monitor.VerboseLog($"Extra conditions prevent spawning. Skipping this area.");
                                continue;
                            }

                            Utility.Monitor.VerboseLog("All extra conditions met. Checking forage types...");

                            List<SavedObject> forageObjects = null; //the list of possible forage objects to spawn in this area today (parsed into SavedObject format)

                            switch (Game1.currentSeason)
                            {
                                case "spring":
                                    if (area.SpringItemIndex != null) //if there's an "override" list set for this area
                                    {
                                        if (area.SpringItemIndex.Length > 0) //if the override includes any items
                                        {
                                            forageObjects = Utility.ParseSavedObjectsFromItemList(area.SpringItemIndex, area.UniqueAreaID); //parse the override index list for this area
                                        }
                                        //if an area index exists but is empty, *do not* use the main index; users may want to disable spawns in this season
                                    }
                                    else if (data.Config.Forage_Spawn_Settings.SpringItemIndex.Length > 0) //if no "override" list exists and the main index list includes any items
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.SpringItemIndex, area.UniqueAreaID); //parse the global index list
                                    }
                                    break;
                                case "summer":
                                    if (area.SummerItemIndex != null)
                                    {
                                        if (area.SummerItemIndex.Length > 0)
                                        {
                                            forageObjects = Utility.ParseSavedObjectsFromItemList(area.SummerItemIndex, area.UniqueAreaID);
                                        }
                                    }
                                    else if (data.Config.Forage_Spawn_Settings.SummerItemIndex.Length > 0)
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.SummerItemIndex, area.UniqueAreaID);
                                    }
                                    break;
                                case "fall":
                                    if (area.FallItemIndex != null)
                                    {
                                        if (area.FallItemIndex.Length > 0)
                                        {
                                            forageObjects = Utility.ParseSavedObjectsFromItemList(area.FallItemIndex, area.UniqueAreaID);
                                        }
                                    }
                                    else if (data.Config.Forage_Spawn_Settings.FallItemIndex.Length > 0)
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.FallItemIndex, area.UniqueAreaID);
                                    }
                                    break;
                                case "winter":
                                    if (area.WinterItemIndex != null)
                                    {
                                        if (area.WinterItemIndex.Length > 0)
                                        {
                                            forageObjects = Utility.ParseSavedObjectsFromItemList(area.WinterItemIndex, area.UniqueAreaID);
                                        }
                                    }
                                    else if (data.Config.Forage_Spawn_Settings.WinterItemIndex.Length > 0)
                                    {
                                        forageObjects = Utility.ParseSavedObjectsFromItemList(data.Config.Forage_Spawn_Settings.WinterItemIndex, area.UniqueAreaID);
                                    }
                                    break;
                            }

                            if (forageObjects == null || forageObjects.Count <= 0)
                            {
                                Utility.Monitor.VerboseLog($"The item index list for this season contains no valid items. Skipping this area.");
                                continue;
                            }

                            Utility.Monitor.VerboseLog($"Valid spawn types: {forageObjects.Count}. Generating today's spawns...");

                            for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                            {
                                //calculate how much forage to spawn today
                                int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, data.Config.Forage_Spawn_Settings.PercentExtraSpawnsPerForagingLevel, Utility.Skills.Foraging);

                                List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned
                                int skippedSpawns = 0; //the number of objects skipped due to their spawn chances

                                //begin to generate forage
                                while (spawnCount > 0) //while more forage should be spawned
                                {
                                    spawnCount--;

                                    //get the total spawn weight of available forage types
                                    int totalWeight = 0;

                                    foreach (SavedObject obj in forageObjects) //for each object in the forage list
                                    {
                                        totalWeight += obj.ConfigItem?.SpawnWeight ?? 1; //increment total weight by this object's spawn weight (default 1)
                                    }

                                    //select a random forage type
                                    SavedObject randomForage = null;

                                    int random = Utility.RNG.Next(0, totalWeight); //get a random integer from 0 to (totalWeight - 1)
                                    for (int f = 0; f < forageObjects.Count; f++) //for each object in the forage list
                                    {
                                        int spawnWeight = forageObjects[f].ConfigItem?.SpawnWeight ?? 1; //get this object's spawn weight (default 1)

                                        if (random < spawnWeight) //if the random number is "within" this object's spawn weight
                                        {
                                            randomForage = forageObjects[f]; //select this object
                                            break; //skip the remaining objects
                                        }
                                        else
                                            random -= spawnWeight; //subtract this object's weight from the random number (then check the next object)
                                    }

                                    double? spawnChance = randomForage.ConfigItem?.PercentChanceToSpawn; //get this object's spawn chance, if provided
                                    if (spawnChance.HasValue && spawnChance.Value < Utility.RNG.Next(100)) //if this object "fails" its chance to spawn
                                    {
                                        skippedSpawns++; //increment skip counter
                                        continue; //skip to the next spawn
                                    }

                                    //create a new saved object based on the randomly selected forage (still using a "blank" tile location)
                                    SavedObject forage = new SavedObject()
                                    {
                                        MapName = locations[x],
                                        Type = randomForage.Type,
                                        Name = randomForage.Name,
                                        ID = randomForage.ID,
                                        DaysUntilExpire = area.DaysUntilSpawnsExpire,
                                        ConfigItem = Utility.Clone(randomForage.ConfigItem) //use a separate copy of this
                                    };

                                    //if this object has contents with spawn chances, process them
                                    if (forage.ConfigItem?.Contents != null) //if this forage item has contents
                                    {
                                        for (int content = forage.ConfigItem.Contents.Count - 1; content >= 0; content--) //for each of the contents
                                        {
                                            List<SavedObject> contentSave = Utility.ParseSavedObjectsFromItemList(new object[] { forage.ConfigItem.Contents[content] }, area.UniqueAreaID); //parse this into a saved object

                                            double? contentSpawnChance = contentSave[0].ConfigItem?.PercentChanceToSpawn; //get this item's spawn chance, if provided
                                            if (contentSpawnChance.HasValue && contentSpawnChance.Value < Utility.RNG.Next(100)) //if this item "fails" its chance to spawn
                                            {
                                                forage.ConfigItem.Contents.RemoveAt(content); //remove this content from the forage object
                                            }
                                        }
                                    }

                                    spawns.Add(forage); //add it to the list
                                }

                                Utility.Monitor.VerboseLog($"Potential spawns at {locations[x]}: {spawns.Count}");

                                if (skippedSpawns > 0) //if any spawns were skipped due to their spawn chances
                                {
                                    Utility.Monitor.VerboseLog($"Spawns skipped due to spawn chance settings: {skippedSpawns}");
                                }

                                Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns
                            }

                            Utility.Monitor.VerboseLog($"Forage generation complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})");
                        }

                        if (data.Pack != null)
                            Utility.Monitor.VerboseLog($"Forage generation complete for this content pack: {data.Pack.Manifest.Name}");
                        else
                            Utility.Monitor.VerboseLog($"Forage generation complete for this file: data/{Constants.SaveFolderName}.json");
                    }

                    Utility.Monitor.VerboseLog("Forage generation complete.");
                }
                catch (Exception ex)
                {
                    Utility.Monitor.Log($"An error occurred while generating forage and similar items. Some items might fail to spawn. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }
        }
    }
}
