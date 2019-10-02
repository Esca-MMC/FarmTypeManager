using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        private static partial class Generation
        {
            /// <summary>Spawns provided objects where their spawn times match the provided time, then removes them from the provided list.</summary>
            /// <param name="timedSpawns">A list of timed spawn objects to be spawned. Spawned objects will be removed from this list.</param>
            /// <param name="time">An in-game time value. If provided, only objects with matching SpawnTime values will be spawned.</param>
            public static void SpawnTimedSpawns(List<List<TimedSpawn>> timedSpawns, StardewTime? time = null)
            {
                Utility.Monitor.VerboseLog($"Spawning objects set to appear at time: {(int?)time}");

                int spawned = 0; //tracks the number of objects spawned during this process
                bool filter(TimedSpawn spawn) => spawn.SavedObject.SpawnTime == time; //define a filter that is true when a TimedSpawn matches the provided time

                for (int x = timedSpawns.Count - 1; x >= 0; x--) //for each list of spawns (looping backward for removal purposes)
                {
                    List<TimedSpawn> spawns = null; //refer to the current list as "spawns"

                    if (time.HasValue == false) //if no time was provided
                    {
                        spawns = timedSpawns[x]; //don't filter the list
                    }
                    else //if a time was provided
                    {
                        spawns = timedSpawns[x].Where(filter).ToList(); //get a list of spawns with matching times
                        timedSpawns[x].RemoveAll(filter); //remove the matching spawns from the original list
                    }

                    if (timedSpawns[x].Count <= 0) //if the list is now empty
                    {
                        timedSpawns.RemoveAt(x); //remove the list
                    }

                    if (spawns.Count <= 0) //if nothing in the list had a matching time
                    {
                        continue; //skip to the next list
                    }

                    //validate the "only spawn if a player is present" setting
                    if (spawns[0].SpawnArea.SpawnTiming.OnlySpawnIfAPlayerIsPresent)
                    {
                        GameLocation location = Game1.getLocationFromName(spawns[0].SpawnArea.MapName); //get this area's lcoation
                        FarmerCollection farmers = Game1.getOnlineFarmers(); //get all active players

                        bool playerIsPresent = false;
                        foreach (Farmer farmer in farmers)
                        {
                            if (farmer.currentLocation == location) //if this farmer is at the current location
                            {
                                playerIsPresent = true;
                                break;
                            }
                        }

                        if (!playerIsPresent) //if no players are present
                        {
                            Utility.Monitor.Log($"Skipping spawns for this area because no players are present: {spawns[0].SpawnArea.UniqueAreaID} ({spawns[0].SpawnArea.MapName})", LogLevel.Trace);
                            continue; //skip to the next list
                        }
                    }

                    bool isLarge = spawns[0].SavedObject.Type == SavedObject.ObjectType.LargeObject; //whether these spawns are large (2x2 tiles); checked via the first spawn in the list
                    int[] customTiles = { }; //the set of custom tiles to use (to be selected based on the spawn object's type)
                    switch (spawns[0].SavedObject.Type)
                    {
                        case SavedObject.ObjectType.Forage:
                            customTiles = spawns[0].FarmData.Config.Forage_Spawn_Settings.CustomTileIndex;
                            break;
                        case SavedObject.ObjectType.LargeObject:
                            customTiles = spawns[0].FarmData.Config.Large_Object_Spawn_Settings.CustomTileIndex;
                            break;
                        case SavedObject.ObjectType.Ore:
                            customTiles = spawns[0].FarmData.Config.Ore_Spawn_Settings.CustomTileIndex;
                            break;
                        case SavedObject.ObjectType.Monster:
                            customTiles = spawns[0].FarmData.Config.Monster_Spawn_Settings.CustomTileIndex;
                            break;
                    }

                    //generate a new list of valid tiles for this spawn area
                    List<Vector2> tiles = Utility.GenerateTileList(spawns[0].SpawnArea, spawns[0].FarmData.Save, spawns[0].FarmData.Config.QuarryTileIndex, customTiles, isLarge);

                    for (int y = spawns.Count - 1; y >= 0; y--) //for each object to be spawned (looping backward for removal purposes)
                    {
                        Vector2? chosenTile = null;
                        while (tiles.Count > 0 && !chosenTile.HasValue) //while potential tiles exist & a valid tile has not been chosen yet
                        {
                            int randomIndex = Utility.RNG.Next(tiles.Count); //get the array index for a random valid tile
                            if (Utility.IsTileValid(spawns[y].SpawnArea, tiles[randomIndex], isLarge)) //if this tile is valid
                            {
                                chosenTile = tiles[randomIndex]; //choose this tile
                            }
                            tiles.RemoveAt(randomIndex); //remove the tile from the list
                        }

                        if (!chosenTile.HasValue) //if no remaining tiles were valid
                        {
                            break; //skip the rest of this spawn list
                        }

                        spawns[y].SavedObject.Tile = chosenTile.Value; //apply the random tile to this spawn  

                        //spawn the object based on its type
                        switch (spawns[y].SavedObject.Type)
                        {
                            case SavedObject.ObjectType.Forage:
                                Utility.SpawnForage(spawns[y].SavedObject.ID.Value, Game1.getLocationFromName(spawns[y].SpawnArea.MapName), spawns[y].SavedObject.Tile); //spawn forage
                                break;
                            case SavedObject.ObjectType.LargeObject:
                                Utility.SpawnLargeObject(spawns[y].SavedObject.ID.Value, (Farm)Game1.getLocationFromName(spawns[y].SpawnArea.MapName), spawns[y].SavedObject.Tile); //spawn large object
                                break;
                            case SavedObject.ObjectType.Ore:
                                int? oreID = Utility.SpawnOre(spawns[y].SavedObject.Name, Game1.getLocationFromName(spawns[y].SpawnArea.MapName), spawns[y].SavedObject.Tile); //spawn ore and get its ID if successful
                                if (oreID.HasValue) //if the ore spawned successfully (i.e. generated an ID)
                                {
                                    spawns[y].SavedObject.ID = oreID.Value; //record this spawn's ID
                                }
                                break;
                            case SavedObject.ObjectType.Monster:
                                int? monID = Utility.SpawnMonster(spawns[y].SavedObject.MonType, Game1.getLocationFromName(spawns[y].SpawnArea.MapName), spawns[y].SavedObject.Tile, spawns[y].SpawnArea.UniqueAreaID); //spawn monster and get its ID if successful
                                if (monID.HasValue)
                                {
                                    spawns[y].SavedObject.ID = monID.Value; //record this spawn's ID
                                }
                                break;
                        }
                        spawned++; //increment the spawn tracker

                        if (spawns[y].SavedObject.ID.HasValue && spawns[y].SavedObject.DaysUntilExpire.HasValue) //if this object spawned successfully and has an expiration date
                        {
                            spawns[y].FarmData.Save.SavedObjects.Add(spawns[y].SavedObject); //add the spawn to the relevant save data
                        }
                    }
                }
                Utility.Monitor.VerboseLog($"Spawn process complete for time: {(int)time}. Objects spawned: {spawned}");
            }
        }
    }
}