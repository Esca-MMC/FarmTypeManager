using StardewModdingAPI;
using System;
using System.Collections.Generic;

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
                try
                {
                    foreach (FarmData data in Utility.FarmDataList)
                    {
                        if (data.Config?.MonsterSpawnEnabled == true && data.Config.Monster_Spawn_Settings != null)
                        {
                            if (data.Pack != null)
                                Utility.Monitor.Log($"Generating monsters for content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                            else
                                Utility.Monitor.Log($"Generating monsters for local file: data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                        }
                        else
                        {
                            if (data.Pack != null)
                                Utility.Monitor.VerboseLog($"Monster generation is disabled for content pack: {data.Pack.Manifest.Name}");
                            else
                                Utility.Monitor.VerboseLog($"Monster generation is disabled for local file: data/{Constants.SaveFolderName}.json");

                            continue;
                        }

                        foreach (MonsterSpawnArea area in data.Config.Monster_Spawn_Settings.Areas)
                        {
                            Utility.Monitor.VerboseLog($"Checking monster settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})");

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

                            Utility.Monitor.VerboseLog($"All extra conditions met. Validating list of monster types...");

                            //validate the provided monster types
                            List<MonsterType> validMonsterTypes = Utility.ValidateMonsterTypes(area.MonsterTypes, area.UniqueAreaID);

                            if (validMonsterTypes.Count <= 0)
                            {
                                Utility.Monitor.VerboseLog($"The monster list contains no valid types. Skipping this area.");
                                continue;
                            }

                            Utility.Monitor.VerboseLog($"Valid monster types: {validMonsterTypes.Count}. Generating today's spawns...");

                            for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                            {
                                //calculate how many monsters to spawn today
                                int spawnCount = Utility.RNG.Next(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay + 1); //random number from min to max

                                Utility.Monitor.VerboseLog($"Potential spawns at {locations[x]}: {spawnCount}.");

                                List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned

                                //begin to generate monsters
                                while (spawnCount > 0) //while more monsters should be spawned
                                {
                                    spawnCount--;

                                    //get the total spawn weight of valid monster types
                                    int totalWeight = 0;
                                    foreach (MonsterType type in validMonsterTypes) //for each valid monster type
                                    {
                                        if (type.Settings.ContainsKey("SpawnWeight")) //if a custom spawn weight was provided
                                        {
                                            totalWeight += Convert.ToInt32(type.Settings["SpawnWeight"]);
                                        }
                                        else //if no spawn weight was provided
                                        {
                                            totalWeight += 1;
                                        }
                                    }

                                    //select a random monster using spawn weights
                                    MonsterType randomMonster = null;
                                    int random = Utility.RNG.Next(0, totalWeight); //get a random integer from 0 to (totalWeight - 1)

                                    for (int m = 0; m < validMonsterTypes.Count; m++) //for each valid monster type
                                    {
                                        int spawnWeight = 1; //default to 1
                                        if (validMonsterTypes[m].Settings.ContainsKey("SpawnWeight")) //if a spawn weight was provided
                                        {
                                            spawnWeight = Convert.ToInt32(validMonsterTypes[m].Settings["SpawnWeight"]); //use it
                                        }

                                        if (random < spawnWeight) //if this monster type is selected
                                        {
                                            randomMonster = Utility.Clone(validMonsterTypes[m]); //get the selected monster type (cloned for later use as a unique instance)
                                            break;
                                        }
                                        else //if this monster type is not selected
                                        {
                                            random -= spawnWeight; //subtract this item's weight from the random number
                                        }
                                    }

                                    //create a saved object representing this spawn (with a "blank" tile location)
                                    SavedObject saved = new SavedObject()
                                    {
                                        MapName = locations[x],
                                        Type = SavedObject.ObjectType.Monster,
                                        DaysUntilExpire = area.DaysUntilSpawnsExpire ?? 1,
                                        MonType = randomMonster
                                    };
                                    spawns.Add(saved); //add it to the list
                                }

                                Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns
                            }

                            Utility.Monitor.VerboseLog($"Monster spawn process complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})");
                        }

                        if (data.Pack != null)
                            Utility.Monitor.VerboseLog($"Monster generation complete for this content pack: {data.Pack.Manifest.Name}");
                        else
                            Utility.Monitor.VerboseLog($"Monster generation complete for this file: data/{Constants.SaveFolderName}.json");
                    }

                    Utility.Monitor.VerboseLog("Monster generation complete.");
                }
                catch (Exception ex)
                {
                    Utility.Monitor.Log($"An error occurred while generating monsters. Some monsters might fail to spawn. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }
        }
    }
}
