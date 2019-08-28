using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates ore and places it on the specified map and tile.</summary>
            /// <param name="monsterType">The monster type's name and an optional dictionary of monster-specific settings.</param>
            /// <param name="location">The GameLocation where the monster should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the monster should be spawned.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>Returns false if spawn could not be attempted.</returns>
            public static bool SpawnMonster(MonsterType monsterType, GameLocation location, Vector2 tile, string areaID = "")
            {
                Monster monster = null; //an instatiated monster, to be spawned into the world later

                int? mineLevel = null; //used by some monster types to determine subtype or other properties
                Color? color = null; //the monster's color (used by slimes)

                if (monsterType.Settings != null) //if settings were provided
                {
                    if (monsterType.Settings.ContainsKey("MineLevel")) //if mineLevel was provided
                    {
                        mineLevel = Convert.ToInt32(monsterType.Settings["MineLevel"]);
                    }

                    if (monsterType.Settings.ContainsKey("Color")) //if color was provided
                    {
                        string[] colorText = ((string)monsterType.Settings["Color"]).Trim().Split(' '); //split the setting string into strings for each number
                        int[] colorRGB = new int[] { Convert.ToInt32(colorText[0]), Convert.ToInt32(colorText[1]), Convert.ToInt32(colorText[2]) }; //convert the strings into numbers

                        color = new Color(colorRGB[0], colorRGB[1], colorRGB[2]); //set the color variable
                    }
                    else if (monsterType.Settings.ContainsKey("MinColor") && monsterType.Settings.ContainsKey("MaxColor")) //if color wasn't provided, but mincolor & maxcolor were
                    {
                        string[] minColorText = ((string)monsterType.Settings["MinColor"]).Trim().Split(' '); //split the setting string into strings for each number
                        int[] minColorRGB = new int[] { Convert.ToInt32(minColorText[0]), Convert.ToInt32(minColorText[1]), Convert.ToInt32(minColorText[2]) }; //convert the strings into numbers

                        string[] maxColorText = ((string)monsterType.Settings["MaxColor"]).Trim().Split(' '); //split the setting string into strings for each number
                        int[] maxColorRGB = new int[] { Convert.ToInt32(maxColorText[0]), Convert.ToInt32(maxColorText[1]), Convert.ToInt32(maxColorText[2]) }; //convert the strings into numbers

                        //pick random RGB color between min and max
                        int r = RNG.Next(minColorRGB[0], maxColorRGB[0] + 1);
                        int g = RNG.Next(minColorRGB[1], maxColorRGB[1] + 1);
                        int b = RNG.Next(minColorRGB[2], maxColorRGB[2] + 1);

                        color = new Color(r, g, b); //set the color variable
                    }
                }

                //create a new monster based on the provided name & apply type-specific settings
                switch (monsterType.MonsterName.ToLower()) //avoid any casing issues in method calls by making this lower-case
                {
                    case "bat":
                        monster = new Bat(tile, mineLevel ?? 0);
                        break;
                    case "bigslime":
                    case "big slime":
                        monster = new BigSlime(tile, mineLevel ?? 0);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlime)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "bug":
                        monster = new Bug(tile, mineLevel ?? 0);
                        break;
                    case "carbonghost":
                    case "carbon ghost":
                        monster = new Ghost(tile, "Carbon Ghost");
                        break;
                    case "duggy":
                        monster = new Duggy(tile);
                        //TODO: fix duggy destruction (tile index change) & refusal to unborrow (iirc; something to do with the base behavior?)
                        //      maybe also fix the hardcoded damage reset caused by its attack behavior
                        //
                        //      and while at it, remember to fix flying monsters in general
                        //      (override their "draw above most layers" or w/e thing to call the "draw above all" one afterward? see Farm.cs)
                        break;
                    case "dust":
                    case "spirit":
                    case "dustspirit":
                    case "dust spirit":
                        monster = new DustSpirit(tile);
                        break;
                    case "fly":
                        if (mineLevel.HasValue && mineLevel > 120) //note: minelevel normally isn't involved with flies, but is used here as shorthand
                        {
                            monster = new Fly(tile, true); //spawn a hard fly
                        }
                        else
                        {
                            monster = new Fly(tile); //spawn a normal fly
                        }
                        break;
                    case "ghost":
                        monster = new Ghost(tile);
                        break;
                    case "slime":
                    case "greenslime":
                    case "green slime":
                        if (mineLevel.HasValue) //if minelevel was provided
                        {
                            monster = new GreenSlime(tile, mineLevel.Value); //spawn a slime based on minelevel
                            if (color.HasValue) //if color was also provided
                            {
                                ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                            }
                        }
                        else if (color.HasValue) //if minelevel wasn't provided but color was
                        {
                            monster = new GreenSlime(tile, color.Value); //spawn a slime based on color
                        }
                        else //if minelevel and color weren't provided
                        {
                            monster = new GreenSlime(tile); //spawn a default slime
                        }
                        break;
                    case "grub":
                        if (mineLevel.HasValue && mineLevel > 120) //note: minelevel normally isn't directly involved with grubs, but is used here as shorthand
                        {
                            monster = new Grub(tile, true); //spawn a hard grub
                        }
                        else
                        {
                            monster = new Grub(tile); //spawn a normal grub
                        }
                        break;
                    case "iridiumcrab":
                    case "iridium crab":
                        monster = new RockCrab(tile, "Iridium Crab");
                        break;
                    case "lavacrab":
                    case "lava crab":
                        monster = new LavaCrab(tile);
                        break;
                    case "metalhead":
                    case "metal head":
                        if (!mineLevel.HasValue || mineLevel < 40) //if minelevel wasn't provided or is less than 40
                        {
                            monster = new MetalHead(tile, 0); //spawn the first subtype
                        }
                        else if (mineLevel < 80) //if minelevel is 40-79
                        {
                            monster = new MetalHead(tile, 40); //spawn the second subtype
                        }
                        else
                        {
                            monster = new MetalHead(tile, 80); //spawn the third subtype
                        }

                        if (color.HasValue) //if color was provided
                        {
                            ((MetalHead)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "mummy":
                        monster = new Mummy(tile);
                        break;
                    case "rockcrab":
                    case "rock crab":
                        monster = new RockCrab(tile);
                        break;
                    case "stonegolem":
                    case "stone golem":
                        monster = new RockGolem(tile);
                        if (mineLevel.HasValue) //if minelevel was provided
                        {
                            //perform a modified version of the RockGolem(position, mineArea) constructor, which sets default HP and damage
                            if (mineLevel >= 80)
                            {
                                monster.DamageToFarmer *= 2;
                                monster.Health = (int)((double)monster.Health * 2.5);
                            }
                            else if (mineLevel >= 40)
                            {
                                monster.DamageToFarmer = (int)((double)monster.DamageToFarmer * 1.5);
                                monster.Health = (int)((double)monster.Health * 1.75);
                            }
                        }
                        break;
                    case "serpent":
                        monster = new Serpent(tile);
                        break;
                    case "brute":
                    case "shadowbrute":
                    case "shadow brute":
                        monster = new ShadowBrute(tile);
                        break;
                    case "shaman":
                    case "shadowshaman":
                    case "shadow shaman":
                        monster = new ShadowShaman(tile);
                        break;
                    case "skeleton":
                        monster = new Skeleton(tile);
                        break;
                    case "squid":
                    case "kid":
                    case "squidkid":
                    case "squid kid":
                        monster = new SquidKid(tile);
                        break;
                    case "wildernessgolem":
                    case "wilderness golem":
                        monster = new RockGolem(tile, Game1.player.CombatLevel); //note: this uses the main player's combat level to imitate the base game
                        break;
                    default: break;
                }

                if (monster == null)
                {
                    Monitor.Log($"The monster to be spawned (\"{monsterType.MonsterName}\") doesn't match any known monster types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                    return false;
                }

                ApplyMonsterSettings(monster, monsterType.Settings, areaID); //adjust the monster based on any other provided optional settings

                //spawn the completed monster at the target location
                Monitor.VerboseLog($"Spawning monster. Type: {monsterType.MonsterName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                monster.currentLocation = location;
                monster.setTileLocation(tile);
                location.addCharacter(monster);
                return true;
            }
        }
    }
}