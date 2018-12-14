using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods involved in spawning objects into the game.</summary> 
        static class ObjectSpawner
        {
            
            /// <summary>Generates foraged items in the game based on the current player's config settings.</summary>
            /// <param name="config">Mod configuration settings for the current player.</param>
            public static void ForageGeneration(FarmConfig config)
            {
                if (config.ForageSpawn_Enabled != true) { return; } //if forage spawn is disabled, don't do anything

                Random rng = new Random(); //random number generator ("Game1.random" exists, but causes some undesired spawn behavior; using this for now...)

                foreach (SpawnArea area in config.ForageSpawn.Areas) //for each forage spawn area described in the player config file
                {
                    List<Vector2> validTiles = new List<Vector2>(); //list of all open, valid tiles for new spawns on the current map

                    foreach (string type in area.AutoSpawnTerrainTypes) //loop to auto-detect valid tiles based on various types of terrain
                    {
                        if (type.Equals("custom", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "custom" tile index list
                        {
                            validTiles.AddRange(Utility.GetTilesByIndex(area.MapName, config.ForageSpawn.CustomTileIndex));
                        }
                        else  //add any tiles with properties matching "type" (e.g. tiles with the "Diggable" property, "Grass" type, etc; if "type is "All", this will just add every valid tile)
                        {
                            validTiles.AddRange(Utility.GetTilesByProperty(area.MapName, type));
                        }
                    }
                    foreach (string include in area.IncludeSpawnAreas) //check for valid tiles in each "include" zone for the area
                    {
                        validTiles.AddRange(Utility.GetTilesByVectorString(area.MapName, include));
                    }

                    validTiles = validTiles.Distinct().ToList(); //remove any duplicate tiles from the list

                    foreach (string exclude in area.ExcludeSpawnAreas) //check for valid tiles in each "exclude" zone for the area (validity doesn't really matter in this part, but should be slightly faster)
                    {
                        List<Vector2> excludedTiles = Utility.GetTilesByVectorString(area.MapName, exclude); //get list of excluded tiles
                        validTiles.RemoveAll(excludedTiles.Contains); //remove any previously valid tiles that match the excluded area
                    }

                    //calculate how much forage to spawn today
                    int spawnCount = rng.Next(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay + 1); //random number from min to max (higher number is exclusive, so +1 to adjust for it)

                    //calculate foraging skill multiplier bonus
                    double skillMultiplier = config.ForageSpawn.PercentExtraItemsPerForagingLevel;
                    skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. default config is "10" (10% per level) so it converts to "0.1"
                    int highestSkillLevel = 0; //highest foraging skill level among all existing farmers, not just the host
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        highestSkillLevel = Math.Max(highestSkillLevel, farmer.ForagingLevel); //record the new level if it's higher than before
                    }
                    skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. with default config: "1.0" at level 0, "1.7" at level 7, etc

                    //calculate final forage amount
                    spawnCount = (int)Math.Round(skillMultiplier * (double)spawnCount);

                    //begin to spawn forage
                    while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for forage & still forage to be spawned
                    {
                        //this section spawns 1 forage item at a random valid location

                        spawnCount--; //reduce by 1, since one will be spawned
                        int randomIndex = rng.Next(validTiles.Count); //get the array index for a random valid tile
                        Vector2 randomTile = validTiles[randomIndex]; //get the random tile's x,y coordinates
                        validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed by forage now

                        int randomForageType = -1; //will stay at -1 if the current season has no forage items listed, or be set to a random item's index number
                        switch (Game1.currentSeason)
                        {
                            case "spring":
                                if (config.ForageSpawn.SpringItemIndex.Length > 0)
                                {
                                    randomForageType = config.ForageSpawn.SpringItemIndex[rng.Next(config.ForageSpawn.SpringItemIndex.Length)]; //get a random index from the spring list
                                }
                                break;
                            case "summer":
                                if (config.ForageSpawn.SummerItemIndex.Length > 0)
                                {
                                    randomForageType = config.ForageSpawn.SummerItemIndex[rng.Next(config.ForageSpawn.SummerItemIndex.Length)];
                                }
                                break;
                            case "fall":
                                if (config.ForageSpawn.FallItemIndex.Length > 0)
                                {
                                    randomForageType = config.ForageSpawn.FallItemIndex[rng.Next(config.ForageSpawn.FallItemIndex.Length)];
                                }
                                break;
                            case "winter":
                                if (config.ForageSpawn.WinterItemIndex.Length > 0)
                                {
                                    randomForageType = config.ForageSpawn.WinterItemIndex[rng.Next(config.ForageSpawn.WinterItemIndex.Length)];
                                }
                                break;
                        }
                        if (randomForageType != -1) //if the forage type seems valid
                        {
                            //this method call is based on code from SDV's DayUpdate() in Farm.cs, as of SDV 1.3.27
                            Game1.getLocationFromName(area.MapName).dropObject(new StardewValley.Object(randomTile, randomForageType, (string)null, false, true, false, true), randomTile * 64f, Game1.viewport, true, (Farmer)null);
                        }
                    }
                }
            }
            
            /// <summary>Generates ore in the game based on the current player's config settings.</summary>
            /// <param name="config">Mod configuration settings for the current player.</param>
            public static void OreGeneration(FarmConfig config)
            {
                if (config.OreSpawn_Enabled != true) { return; } //if ore spawn is disabled, don't do anything

                Random rng = new Random(); //random number generator ("Game1.random" exists, but causes some undesired spawn behavior; using this for now...)

                foreach (OreSpawnArea area in config.OreSpawn.Areas) //for each ore spawn area described in the player config file
                {
                    List<Vector2> validTiles = new List<Vector2>(); //list of all open, valid tiles for new spawns on the current map

                    foreach (string type in area.AutoSpawnTerrainTypes) //loop to auto-detect valid tiles based on various types of terrain
                    {
                        if (type.Equals("quarry", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "quarry" tile index list
                        {
                            validTiles.AddRange(Utility.GetTilesByIndex(area.MapName, config.OreSpawn.QuarryTileIndex));
                        }
                        else if (type.Equals("custom", StringComparison.OrdinalIgnoreCase)) //add tiles matching the "custom" tile index list
                        {
                            validTiles.AddRange(Utility.GetTilesByIndex(area.MapName, config.OreSpawn.CustomTileIndex));
                        }
                        else  //add any tiles with properties matching "type" (e.g. tiles with the "Diggable" property, "Grass" type, etc; if "type is "All", this will just add every valid tile)
                        {
                            validTiles.AddRange(Utility.GetTilesByProperty(area.MapName, type));
                        }
                    }
                    foreach (string include in area.IncludeSpawnAreas) //check for valid tiles in each "include" zone for the area
                    {
                        validTiles.AddRange(Utility.GetTilesByVectorString(area.MapName, include));
                    }

                    validTiles = validTiles.Distinct().ToList(); //remove any duplicate tiles from the list

                    foreach (string exclude in area.ExcludeSpawnAreas) //check for valid tiles in each "exclude" zone for the area (validity doesn't really matter in this part, but should be slightly faster)
                    {
                        List<Vector2> excludedTiles = Utility.GetTilesByVectorString(area.MapName, exclude); //get list of excluded tiles
                        validTiles.RemoveAll(excludedTiles.Contains); //remove any previously valid tiles that match the excluded area
                    }

                    //calculate how much ore to spawn today
                    int spawnCount = rng.Next(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay + 1); //random number from min to max (higher number is exclusive, so +1 to adjust for it)

                    //calculate mining skill multiplier bonus
                    double skillMultiplier = config.OreSpawn.PercentExtraOrePerMiningLevel;
                    skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. default config is "10" (10% per level) so it converts to "0.1"
                    int highestSkillLevel = 0; //highest mining skill level among all existing farmers, not just the host
                    foreach (Farmer farmer in Game1.getAllFarmers())
                    {
                        highestSkillLevel = Math.Max(highestSkillLevel, farmer.MiningLevel); //record the new level if it's higher than before
                    }
                    skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. with default config: "1.0" at level 0, "1.7" at level 7, etc

                    //calculate final ore amount
                    spawnCount = (int)Math.Round(skillMultiplier * (double)spawnCount);

                    //figure out which config section to use (if the spawn area's data is null, use the "global" data instead)
                    Dictionary<string, int> skillReq = area.SkillRequired ?? config.OreSpawn.MiningSkillRequired;
                    Dictionary<string, int> startChance = area.StartingSpawnChance ?? config.OreSpawn.StartingChance;
                    Dictionary<string, int> tenChance = area.LevelTenSpawnChance ?? config.OreSpawn.LevelTenChance;
                    //also use the "global" data if the area data is non-null but empty (which can happen accidentally when the json file is manually edited)
                    if (skillReq.Count < 1)
                    {
                        skillReq = config.OreSpawn.MiningSkillRequired;
                    }
                    if (startChance.Count < 1)
                    {
                        startChance = config.OreSpawn.StartingChance;
                    }
                    if (tenChance.Count < 1)
                    {
                        tenChance = config.OreSpawn.LevelTenChance;
                    }

                    //calculate the final spawn chance for each type of ore
                    Dictionary<string, int> oreChances = AdjustedSpawnChances(highestSkillLevel, skillReq, startChance, tenChance);
                    
                    if (oreChances.Count < 1) { return; } //if there's no chance of spawning any ore for some reason, just stop here

                    //begin to spawn ore
                    int randomIndex;
                    Vector2 randomTile;
                    int randomOreNum;
                    while (validTiles.Count > 0 && spawnCount > 0) //while there's still open space for ore & still ore to be spawned
                    {
                        //this section spawns 1 ore at a random valid location

                        spawnCount--; //reduce by 1, since one will be spawned
                        randomIndex = rng.Next(validTiles.Count); //get the array index for a random tile
                        randomTile = validTiles[randomIndex]; //get the tile's x,y coordinates
                        validTiles.RemoveAt(randomIndex); //remove the tile from the list, since it will be obstructed by ore now

                        int totalWeight = 0; //the upper limit for the random number that picks ore type (i.e. the sum of all ore chances)
                        foreach (KeyValuePair<string, int> ore in oreChances)
                        {
                            totalWeight += ore.Value; //sum up all the ore chances
                        }
                        randomOreNum = rng.Next(totalWeight); //generate random number from 0 to [totalWeight - 1]
                        foreach (KeyValuePair<string, int> ore in oreChances)
                        {
                            if (randomOreNum < ore.Value) //this ore "wins"
                            {
                                SpawnOre(ore.Key, area.MapName, randomTile);
                                break;
                            }
                            else //this ore "loses"
                            {
                                randomOreNum -= ore.Value; //subtract this ore's chance from the random number before moving to the next one
                            }
                        }
                    }
                }
            }

            /// <summary>Generates ore and places it on the specified map and tile.</summary>
            /// <param name="oreName">A string representing the name of the ore type to be spawned, e.g. "</param>
            /// <param name="mapName">The name of the GameLocation where the ore should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            private static void SpawnOre(string oreName, string mapName, Vector2 tile)
            {
                Random rng = new Random();
                StardewValley.Object ore = null; //ore object, to be spawned into the world later
                switch (oreName.ToLower()) //avoid any casing issues in method calls by making this lower-case
                {
                    case "stone":
                        ore = new StardewValley.Object(tile, 668 + (rng.Next(2) * 2), 1); //either of the two random stones spawned in the vanilla hilltop quarry
                        ore.MinutesUntilReady = 2; //durability, i.e. number of hits with basic pickaxe required to break the ore (each pickaxe level being +1 damage)
                        break;
                    case "geode":
                        ore = new StardewValley.Object(tile, 75, 1); //"regular" geode rock, as spawned on vanilla hilltop quarries 
                        ore.MinutesUntilReady = 3;
                        break;
                    case "frozengeode":
                        ore = new StardewValley.Object(tile, 76, 1); //frozen geode rock
                        ore.MinutesUntilReady = 5;
                        break;
                    case "magmageode":
                        ore = new StardewValley.Object(tile, 77, 1); //magma geode rock
                        ore.MinutesUntilReady = 8; //TODO: replace this guess w/ actual vanilla durability
                        break;
                    case "gem":
                        ore = new StardewValley.Object(tile, (rng.Next(7) + 1) * 2, "Stone", true, false, false, false); //any of the possible gem rocks
                        ore.MinutesUntilReady = 5; //based on "gemstone" durability, but applies to every type for simplicity's sake
                        break;
                    case "copper":
                        ore = new StardewValley.Object(tile, 751, 1); //copper ore
                        ore.MinutesUntilReady = 3;
                        break;
                    case "iron":
                        ore = new StardewValley.Object(tile, 290, 1); //iron ore
                        ore.MinutesUntilReady = 4;
                        break;
                    case "gold":
                        ore = new StardewValley.Object(tile, 764, 1); //gold ore
                        ore.MinutesUntilReady = 8;
                        break;
                    case "iridium":
                        ore = new StardewValley.Object(tile, 765, 1); //iridium ore
                        ore.MinutesUntilReady = 16; //TODO: confirm this is still the case (it's based on SDV 1.11 code)
                        break;
                    case "mystic":
                        ore = new StardewValley.Object(tile, 46, "Stone", true, false, false, false); //mystic ore, i.e. high-end cavern rock with iridium + gold
                        ore.MinutesUntilReady = 16; //TODO: replace this guess w/ actual vanilla durability
                        break;
                    default: break;
                }

                if (ore != null)
                {
                    GameLocation loc = Game1.getLocationFromName(mapName);
                    loc.setObject(tile, ore); //actually spawn the ore object into the world
                }
                else
                {
                    Utility.Monitor.Log($"The ore to be spawned (\"{oreName}\") doesn't match any known ore types. Make sure that name isn't misspelled in your player config file.", LogLevel.Info);
                }

                return;
            }

            /// <summary>Produces a dictionary containing the final, adjusted spawn chance of each item in the provided dictionaries. (Part of the convoluted object spawning process for ore.)</summary>
            /// <param name="skillLevel">The players' highest level in the related skill (e.g. Mining for ore spawn chances).</param>
            /// <param name="skillRequired">A dictionary of object names and the skill level required to spawn them.</param>
            /// <param name="startChances">A dictionary of object names and their weighted chances to spawn at their lowest required skill level (e.g. chance of spawning stone if you're level 0).</param>
            /// <param name="maxChances">A dictionary of object names and their weighted chances to spawn at skill level 10.</param>
            /// <returns></returns>
            private static Dictionary<string, int> AdjustedSpawnChances(int skillLevel, Dictionary<string, int> skillRequired, Dictionary<string, int> startChances, Dictionary<string, int> maxChances)
            {
                Dictionary<string, int> adjustedChances = new Dictionary<string, int>();

                foreach (KeyValuePair<string, int> objType in skillRequired)
                {
                    int chance = 0; //chance of spawning this object
                    if (objType.Value > skillLevel)
                    {
                        //skill is too low to spawn this object; leave it at 0%
                    }
                    else if (objType.Value == skillLevel)
                    {
                        chance = startChances[objType.Key]; //skill is the minimum required; use the starting chance
                    }
                    else if (skillLevel >= 10)
                    {
                        chance = maxChances[objType.Key]; //level 10 skill; use the max level chance
                    }
                    else //skill is somewhere in between "starting" and "level 10", so do math to set the chance somewhere in between them (i forgot the term for this kind of averaging, sry)
                    {
                        int count = 0;
                        long chanceMath = 0; //used in case the chances are very large numbers for some reason
                        for (int x = objType.Value; x < 10; x++) //loop from [minimum skill level for this object] to [max level - 1], for vague math reasons
                        {
                            if (skillLevel > x)
                            {
                                chanceMath += maxChances[objType.Key]; //add level 10 chance
                            }
                            else
                            {
                                chanceMath += startChances[objType.Key]; //add starting chance
                            }
                            count++;
                        }
                        chanceMath = (long)Math.Round((double)chanceMath / (double)count); //divide to get the average
                        chance = Convert.ToInt32(chanceMath); //convert back to a reasonable number range once the math is done
                    }

                    if (chance > 0) //don't bother adding any objects with 0% or negative spawn chance
                    {
                        adjustedChances.Add(objType.Key, chance); //add the object name & chance to the list of adjusted chances
                    }
                }

                return adjustedChances;
            }
        }
    }
}
