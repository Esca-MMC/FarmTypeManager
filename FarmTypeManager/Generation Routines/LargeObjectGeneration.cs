using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class Generation
        {
            /// <summary>Generates large objects (e.g. stumps and logs) in the game based on the current player's config settings.</summary>
            public static void LargeObjectGeneration()
            {
                try
                {
                    foreach (FarmData data in Utility.FarmDataList)
                    {
                        if (data.Config?.LargeObjectSpawnEnabled == true && data.Config.Large_Object_Spawn_Settings != null)
                        {
                            if (data.Pack != null)
                                Utility.Monitor.Log($"Generating large objects for content pack: {data.Pack.Manifest.Name}", LogLevel.Trace);
                            else
                                Utility.Monitor.Log($"Generating large objects for local file: data/{Constants.SaveFolderName}.json", LogLevel.Trace);
                        }
                        else
                        {
                            if (data.Pack != null)
                                Utility.Monitor.VerboseLog($"Large object generation is disabled for content pack: {data.Pack.Manifest.Name}");
                            else
                                Utility.Monitor.VerboseLog($"Large object generation is disabled for local file: data/{Constants.SaveFolderName}.json");

                            continue;
                        }

                        foreach (LargeObjectSpawnArea area in data.Config.Large_Object_Spawn_Settings.Areas)
                        {
                            Utility.Monitor.VerboseLog($"Checking large object settings for this area: \"{area.UniqueAreaID}\" ({area.MapName})");

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

                            Utility.Monitor.VerboseLog("All extra conditions met. Checking large object types...");

                            List<string> objectIDs = Utility.GetLargeObjectIDs(area.ObjectTypes, area.UniqueAreaID); //get a list of index numbers for this area's object types

                            if (objectIDs.Count <= 0)
                            {
                                Utility.Monitor.VerboseLog($"The large object list contains no valid IDs. Skipping this area.");
                                continue;
                            }

                            //find the locations any existing objects (of the listed types)
                            if (area.FindExistingObjectLocations == true //if enabled
                                && locations.Count == 1 //AND only one location was found (building interiors not currently supported)
                                && !locations[0].StartsWith("UndergroundMine", StringComparison.OrdinalIgnoreCase) //AND it's not a mine level
                                && !locations[0].StartsWith("VolcanoDungeon", StringComparison.OrdinalIgnoreCase) //AND it's not a volcano level
                                )
                            {
                                if (data.Save.ExistingObjectLocations.ContainsKey(area.UniqueAreaID)) //if this area already has a list of existing objects (even if it's blank)
                                {
                                    Utility.Monitor.VerboseLog("\"Find existing objects enabled.\" Using saved tile data from a previous search.");
                                }
                                else //if this config+farm hasn't been checked for existing objects yet 
                                {
                                    Utility.Monitor.VerboseLog("\"Find existing objects\" enabled. Finding and recording large object tiles...");

                                    HashSet<int> idsToFind = new HashSet<int>(); //integer IDs for large objects spawned by this area (note: this intentionally excludes giant crops and other subclasses)
                                    foreach (string id in objectIDs)
                                    {
                                        if (int.TryParse(id, out var intID))
                                            idsToFind.Add(intID);
                                    }

                                    List<string> existingObjects = new List<string>(); //any new object location strings to be added to area.IncludeAreas

                                    foreach (ResourceClump clump in Game1.getLocationFromName(locations[0]).resourceClumps) //for each large object at this location
                                    {
                                        if (idsToFind.Contains(clump.parentSheetIndex.Value)) //if its ID matches one spawned by this area
                                        {
                                            existingObjects.Add($"{clump.Tile.X},{clump.Tile.Y};{clump.Tile.X},{clump.Tile.Y}"); //create a string for its tile ("x,y;x,y") and add it to the list
                                        }
                                    }

                                    Utility.Monitor.VerboseLog($"Recorded existing large objects' tiles as spawn locations for this area. Tiles found: {existingObjects.Count}.");

                                    data.Save.ExistingObjectLocations.Add(area.UniqueAreaID, existingObjects.ToArray()); //add the new strings to the save data for the current config+farm
                                }
                            }
                            else
                            {
                                if (!area.FindExistingObjectLocations)
                                {
                                    Utility.Monitor.VerboseLog("\"Find existing objects\" disabled. Skipping.");
                                }
                                else //if this was caused by location-related limitations
                                {
                                    Utility.Monitor.Log("\"Find existing objects\" cannot be used with multiple locations or dynamically loaded maps (e.g. the mines). The setting will be ignored.", LogLevel.Debug);
                                    Utility.Monitor.Log($"Affected area: {area.UniqueAreaID}", LogLevel.Debug);
                                    Utility.Monitor.Log($"Map name: {area.MapName}", LogLevel.Debug);
                                }
                            }

                            Utility.Monitor.VerboseLog($"Valid spawn types: {objectIDs.Count}. Generating today's spawns...");

                            for (int x = 0; x < locations.Count; x++) //for each location matching this area's map name
                            {
                                //calculate how many objects to spawn today
                                int spawnCount = Utility.AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, area.PercentExtraSpawnsPerSkillLevel, (Utility.Skills)Enum.Parse(typeof(Utility.Skills), area.RelatedSkill, true));

                                Utility.Monitor.VerboseLog($"Potential spawns at {locations[x]}: {spawnCount}.");

                                //begin to generate large objects
                                List<SavedObject> spawns = new List<SavedObject>(); //the list of objects to be spawned
                                while (spawnCount > 0) //while more objects should be spawned
                                {
                                    spawnCount--;

                                    string randomObject = objectIDs[Utility.RNG.Next(objectIDs.Count)]; //get a random object ID to spawn

                                    SavedObject saved = new SavedObject() //create a saved object representing this spawn (with a "blank" tile location)
                                    {
                                        MapName = locations[x],
                                        Type = SavedObject.ObjectType.LargeObject,
                                        ID = randomObject,
                                        DaysUntilExpire = area.DaysUntilSpawnsExpire ?? 0
                                    };
                                    spawns.Add(saved); //add it to the list
                                }

                                Utility.PopulateTimedSpawnList(spawns, data, area); //process the listed spawns and add them to Utility.TimedSpawns
                            }

                            Utility.Monitor.VerboseLog($"Large object generation complete for this area: \"{area.UniqueAreaID}\" ({area.MapName})");
                        }

                        if (data.Pack != null)
                            Utility.Monitor.VerboseLog($"Large object generation complete for this content pack: {data.Pack.Manifest.Name}");
                        else
                            Utility.Monitor.VerboseLog($"Large object generation complete for this file: data/{Constants.SaveFolderName}.json");
                    }

                    Utility.Monitor.VerboseLog("Large object generation complete.");
                }
                catch (Exception ex)
                {
                    Utility.Monitor.Log($"An error occurred while generating large objects. Some objects might fail to spawn. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }
        }
    }
}
