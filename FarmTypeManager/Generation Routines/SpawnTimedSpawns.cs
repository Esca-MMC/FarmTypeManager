using System;
using System.Collections.Generic;
using System.IO;
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
            /// <summary>Spawns provided objects where their spawn times match the provided time, then removes them from the provided list.</summary>
            /// <param name="timedSpawns">A list of timed spawn objects to be spawned. Spawned objects will be removed from this list.</param>
            /// <param name="time">An in-game time value. If provided, only objects with matching SpawnTime values will be spawned.</param>
            public static void SpawnTimedSpawns(List<TimedSpawn> timedSpawns, StardewTime? time = null)
            {
                Utility.Monitor.Log($"Spawning objects set to appear at time: {(int?)time}", LogLevel.Trace);

                int spawned = 0; //tracks the number of objects spawned during this process
                List<TimedSpawn> spawns; //a filtered, sorted list of timed spawns

                if (time.HasValue == false) //if no time was provided
                {
                    spawns = timedSpawns; //don't filter the list
                }
                else //if a time was provided
                {
                    bool filter(TimedSpawn spawn) => spawn.SavedObject.SpawnTime == time; //define a filter that is "true" when a TimedSpawn matches the provided time
                    spawns = timedSpawns.Where(filter).ToList(); //get a list of spawns with matching times
                    timedSpawns.RemoveAll(filter); //remove the matching spawns from the original list
                }

                if (spawns.Count <= 0) //if no spawns match the provided time
                {
                    Utility.Monitor.VerboseLog($"Spawn process complete for time: {(int)time}. Objects spawned: {spawned}");
                    return;
                }

                spawns = spawns.OrderBy(spawn => spawn.FarmData.Pack).ThenBy(spawn => spawn.SpawnArea.UniqueAreaID).ToList(); //sort spawns by content pack (if any) and area ID

                List<Vector2> tiles = null; //a list of potential tiles for spawning

                for (int x = 0; x < spawns.Count; x++) //for each filtered, sorted spawn
                {
                    bool isLarge = spawns[x].SavedObject.Type == SavedObject.ObjectType.LargeObject; //whether the spawn is large (2x2 tiles); true if this is a large object

                    if (x == 0 || spawns[x - 1].FarmData.Pack?.Manifest.UniqueID != spawns[x].FarmData.Pack?.Manifest.UniqueID || spawns[x - 1].SpawnArea.UniqueAreaID != spawns[x].SpawnArea.UniqueAreaID) //if this is the first spawn OR the previous spawn was for a different SpawnArea
                    {
                        int[] customTiles = { }; //the set of custom tiles to use (to be selected based on the spawn object's type)
                        switch (spawns[x].SavedObject.Type)
                        {
                            case SavedObject.ObjectType.Forage:
                                customTiles = spawns[x].FarmData.Config.Forage_Spawn_Settings.CustomTileIndex;
                                break;
                            case SavedObject.ObjectType.LargeObject:
                                customTiles = spawns[x].FarmData.Config.Large_Object_Spawn_Settings.CustomTileIndex;
                                break;
                            case SavedObject.ObjectType.Ore:
                                customTiles = spawns[x].FarmData.Config.Ore_Spawn_Settings.CustomTileIndex;
                                break;
                            case SavedObject.ObjectType.Monster:
                                customTiles = spawns[x].FarmData.Config.Monster_Spawn_Settings.CustomTileIndex;
                                break;
                        }

                        tiles = Utility.GenerateTileList(spawns[x].SpawnArea, spawns[x].FarmData.Save, spawns[x].FarmData.Config.QuarryTileIndex, customTiles, isLarge); //generate a new list of valid tiles
                    }

                    Vector2? randomTile = null; //the tile on which this object should be spawned
                    bool tileConfirmed = false; //false until a valid large (2x2) object location is confirmed
                    while (tiles.Count > 0 && !tileConfirmed) //while potential tiles exist & a valid tile has not been chosen yet
                    {
                        int randomIndex = Utility.RNG.Next(tiles.Count); //get the array index for a random valid tile
                        randomTile = tiles[randomIndex]; //get the random tile's x,y coordinates
                        tiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be invalidated
                        tileConfirmed = Utility.IsTileValid(spawns[x].SpawnArea, randomTile.Value, isLarge); //check whether this tile is valid for this object
                    }

                    if (!tileConfirmed) //if no valid tile was found
                    {
                        continue; //skip to the next spawn
                    }

                    spawns[x].SavedObject.Tile = randomTile.Value; //record this spawn's location

                    //spawn the object based on its type
                    switch (spawns[x].SavedObject.Type)
                    {
                        case SavedObject.ObjectType.Forage:
                            Utility.SpawnForage(spawns[x].SavedObject.ID.Value, Game1.getLocationFromName(spawns[x].SpawnArea.MapName), randomTile.Value); //spawn forage
                            break;
                        case SavedObject.ObjectType.LargeObject:
                            Utility.SpawnLargeObject(spawns[x].SavedObject.ID.Value, (Farm)Game1.getLocationFromName(spawns[x].SpawnArea.MapName), randomTile.Value); //spawn large object
                            break;
                        case SavedObject.ObjectType.Ore:
                            int? oreID = Utility.SpawnOre(spawns[x].SavedObject.Name, Game1.getLocationFromName(spawns[x].SpawnArea.MapName), randomTile.Value); //spawn ore and get its ID if successful
                            if (oreID.HasValue) //if the ore spawned successfully (i.e. generated an ID)
                            {
                                spawns[x].SavedObject.ID = oreID.Value; //record this spawn's ID
                            }
                            break;
                        case SavedObject.ObjectType.Monster:
                            int? monID = Utility.SpawnMonster(spawns[x].SavedObject.MonType, Game1.getLocationFromName(spawns[x].SpawnArea.MapName), randomTile.Value, spawns[x].SpawnArea.UniqueAreaID); //spawn monster and get its ID if successful
                            if (monID.HasValue)
                            {
                                spawns[x].SavedObject.ID = monID.Value; //record this spawn's ID
                            }
                            break;
                    }

                    spawned++; //increment the spawn tracker

                    if (spawns[x].SavedObject.ID.HasValue && spawns[x].SavedObject.DaysUntilExpire.HasValue) //if this object spawned successfully and has an expiration date
                    {
                        spawns[x].FarmData.Save.SavedObjects.Add(spawns[x].SavedObject); //add the spawn to the relevant save data
                    }
                }

                Utility.Monitor.VerboseLog($"Spawn process complete for time: {(int)time}. Objects spawned: {spawned}");
            }
        }
    }
}