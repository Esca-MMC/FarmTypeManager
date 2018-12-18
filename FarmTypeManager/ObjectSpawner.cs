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
            public static void ForageGeneration()
            {
                if (Utility.Config.ForageSpawnEnabled != true) { return; } //if forage spawn is disabled, don't do anything

                Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...

                foreach (SpawnArea area in Utility.Config.Forage_Spawn_Settings.Areas) //for each forage spawn area described in the player config file
                {
                    List<Vector2> validTiles = Utility.GenerateTileList(area, Utility.Config.Forage_Spawn_Settings.CustomTileIndex); //calculate a list of valid tiles for forage in this area

                    //calculate how much forage to spawn today
                    int spawnCount = AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, Utility.Config.Forage_Spawn_Settings.PercentExtraItemsPerForagingLevel, Skills.Foraging);

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
                                if (Utility.Config.Forage_Spawn_Settings.SpringItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.SpringItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.SpringItemIndex.Length)]; //get a random index from the spring list
                                }
                                break;
                            case "summer":
                                if (Utility.Config.Forage_Spawn_Settings.SummerItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.SummerItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.SummerItemIndex.Length)];
                                }
                                break;
                            case "fall":
                                if (Utility.Config.Forage_Spawn_Settings.FallItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.FallItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.FallItemIndex.Length)];
                                }
                                break;
                            case "winter":
                                if (Utility.Config.Forage_Spawn_Settings.WinterItemIndex.Length > 0)
                                {
                                    randomForageType = Utility.Config.Forage_Spawn_Settings.WinterItemIndex[rng.Next(Utility.Config.Forage_Spawn_Settings.WinterItemIndex.Length)];
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
            public static void OreGeneration()
            {
                if (Utility.Config.OreSpawnEnabled != true) { return; } //if ore spawn is disabled, don't do anything

                Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...

                foreach (OreSpawnArea area in Utility.Config.Ore_Spawn_Settings.Areas) //for each ore spawn area described in the player config file
                {
                    List<Vector2> validTiles = Utility.GenerateTileList(area, Utility.Config.Ore_Spawn_Settings.CustomTileIndex); //calculate a list of valid tiles for ore in this area

                    //calculate how much ore to spawn today
                    int spawnCount = AdjustedSpawnCount(area.MinimumSpawnsPerDay, area.MaximumSpawnsPerDay, Utility.Config.Ore_Spawn_Settings.PercentExtraOrePerMiningLevel, Skills.Mining);

                    //figure out which config section to use (if the spawn area's data is null, use the "global" data instead)
                    Dictionary<string, int> skillReq = area.MiningLevelRequired ?? Utility.Config.Ore_Spawn_Settings.MiningLevelRequired;
                    Dictionary<string, int> startChance = area.StartingSpawnChance ?? Utility.Config.Ore_Spawn_Settings.StartingSpawnChance;
                    Dictionary<string, int> tenChance = area.LevelTenSpawnChance ?? Utility.Config.Ore_Spawn_Settings.LevelTenSpawnChance;
                    //also use the "global" data if the area data is non-null but empty (which can happen accidentally when the json file is manually edited)
                    if (skillReq.Count < 1)
                    {
                        skillReq = Utility.Config.Ore_Spawn_Settings.MiningLevelRequired;
                    }
                    if (startChance.Count < 1)
                    {
                        startChance = Utility.Config.Ore_Spawn_Settings.StartingSpawnChance;
                    }
                    if (tenChance.Count < 1)
                    {
                        tenChance = Utility.Config.Ore_Spawn_Settings.LevelTenSpawnChance;
                    }

                    //calculate the final spawn chance for each type of ore
                    Dictionary<string, int> oreChances = AdjustedSpawnChances(Skills.Mining, skillReq, startChance, tenChance);
                    
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

            /// <summary>Produces a dictionary containing the final, adjusted spawn chance of each object in the provided dictionaries. (Part of the convoluted object spawning process for ore.)</summary>
            /// <param name="skill">The player skill that affects spawn chances (e.g. Mining for ore spawn chances).</param>
            /// <param name="levelRequired">A dictionary of object names and the skill level required to spawn them.</param>
            /// <param name="startChances">A dictionary of object names and their weighted chances to spawn at their lowest required skill level (e.g. chance of spawning stone if you're level 0).</param>
            /// <param name="maxChances">A dictionary of object names and their weighted chances to spawn at skill level 10.</param>
            /// <returns></returns>
            private static Dictionary<string, int> AdjustedSpawnChances(Skills skill, Dictionary<string, int> levelRequired, Dictionary<string, int> startChances, Dictionary<string, int> maxChances)
            {
                Dictionary<string, int> adjustedChances = new Dictionary<string, int>();

                int skillLevel = 0; //highest skill level among all existing farmers, not just the host
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    skillLevel = Math.Max(skillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                }

                foreach (KeyValuePair<string, int> objType in levelRequired)
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

            /// <summary>Calculates the final number of objects to spawn today in the current spawning process, based on config settings and player levels in a relevant skill.</summary>
            /// <param name="min">Minimum number of objects to spawn today (before skill multiplier).</param>
            /// <param name="max">Maximum number of objects to spawn today (before skill multiplier).</param>
            /// <param name="percent">Additive multiplier for each of the player's levels in the relevant skill (e.g. 10 would represent +10% objects per level).</param>
            /// <param name="skill">Enumerator for the skill on which the "percent" additive multiplier is based.</param>
            /// <returns>The final number of objects to spawn today in the current spawning process.</returns>
            private static int AdjustedSpawnCount(int min, int max, int percent, Skills skill)
            {
                Random rng = new Random(); //DEVNOTE: "Game1.random" exists, but causes some odd spawn behavior; using this for now...
                int spawnCount = rng.Next(min, max + 1); //random number from min to max (higher number is exclusive, so +1 to adjust for it)

                //calculate skill multiplier bonus
                double skillMultiplier = percent;
                skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. default config is "10" (10% per level) so it converts to "0.1"
                int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                }
                skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. with default config: "1.0" at level 0, "1.7" at level 7, etc

                //calculate final forage amount
                skillMultiplier *= spawnCount; //multiply the initial random spawn count by the skill multiplier
                spawnCount = (int)skillMultiplier; //store the integer portion of the current multiplied value (e.g. this is "1" if the multiplier is "1.7")
                double remainder = skillMultiplier - (int)skillMultiplier; //store the decimal portion of the multiplied value (e.g. this is "0.7" if the multiplier is "1.7")

                if (rng.NextDouble() < remainder) //use remainder as a % chance to spawn one extra object (e.g. if the final count would be "1.7", there's a 70% chance of spawning 2 objects)
                {
                    spawnCount++;
                }

                return spawnCount;
            }

            /// <summary>Enumerated list of player skills, in the order used by Stardew's internal code (e.g. Farmer.cs).</summary>
            public enum Skills {Farming, Fishing, Foraging, Mining, Combat, Luck}
        }
    }
}
