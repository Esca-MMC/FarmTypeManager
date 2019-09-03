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
using FarmTypeManager.Monsters;

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

                Color? color = null; //the monster's color (used by specific monster types)

                if (monsterType.Settings != null) //if settings were provided
                {
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
                switch (monsterType.MonsterName.ToLower()) //avoid most casing issues by making this lower-case
                {
                    case "bat":
                        monster = new BatFTM(tile, 0);
                        break;
                    case "frostbat":
                    case "frost bat":
                        monster = new BatFTM(tile, 40);
                        break;
                    case "lavabat":
                    case "lava bat":
                        monster = new BatFTM(tile, 80);
                        break;
                    case "iridiumbat":
                    case "iridium bat":
                        monster = new BatFTM(tile, 171);
                        break;
                    case "bigslime":
                    case "big slime":
                    case "biggreenslime":
                    case "big green slime":
                        monster = new BigSlimeFTM(tile, 0);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "bigblueslime":
                    case "big blue slime":
                    case "bigfrostjelly":
                    case "big frost jelly":
                        monster = new BigSlimeFTM(tile, 40);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "bigredslime":
                    case "big red slime":
                    case "bigredsludge":
                    case "big red sludge":
                        monster = new BigSlimeFTM(tile, 80);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "bigpurpleslime":
                    case "big purple slime":
                    case "bigpurplesludge":
                    case "big purple sludge":
                        monster = new BigSlimeFTM(tile, 121);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "bug":
                        monster = new Bug(tile, 0);
                        break;
                    case "armoredbug":
                    case "armored bug":
                        monster = new Bug(tile, 121);
                        break;
                    case "duggy":
                        monster = new DuggyFTM(tile, true); //TODO: make the moveAnywhere bool into a customizable setting
                        break;
                    case "dust":
                    case "sprite":
                    case "dustsprite":
                    case "dust sprite":
                    case "spirit":
                    case "dustspirit":
                    case "dust spirit":
                        monster = new DustSpirit(tile);
                        break;
                    case "ghost":
                        monster = new GhostFTM(tile);
                        break;
                    case "carbonghost":
                    case "carbon ghost":
                        monster = new GhostFTM(tile, "Carbon Ghost");
                        break;
                    case "slime":
                    case "greenslime":
                    case "green slime":
                        monster = new GreenSlime(tile, 0);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "blueslime":
                    case "blue slime":
                    case "frostjelly":
                    case "frost jelly":
                        monster = new GreenSlime(tile, 40);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "redslime":
                    case "red slime":
                    case "redsludge":
                    case "red sludge":
                        monster = new GreenSlime(tile, 80);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "purpleslime":
                    case "purple slime":
                    case "purplesludge":
                    case "purple sludge":
                        monster = new GreenSlime(tile, 121);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "grub":
                    case "cavegrub":
                    case "cave grub":
                        monster = new GrubFTM(tile, false);
                        break;
                    case "fly":
                    case "cavefly":
                    case "cave fly":
                        monster = new FlyFTM(tile, false);
                        break;
                    case "mutantgrub":
                    case "mutant grub":
                        monster = new GrubFTM(tile, true);
                        break;
                    case "mutantfly":
                    case "mutant fly":
                        monster = new FlyFTM(tile, true);
                        break;
                    case "metalhead":
                    case "metal head":
                        monster = new MetalHead(tile, 0);
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
                    case "lavacrab":
                    case "lava crab":
                        monster = new LavaCrab(tile);
                        break;
                     case "iridiumcrab":
                    case "iridium crab":
                        monster = new RockCrab(tile, "Iridium Crab");
                        break;
                    case "rockgolem":
                    case "rock golem":
                    case "stonegolem":
                    case "stone golem":
                        monster = new RockGolemFTM(tile);
                        break;
                    case "wildernessgolem":
                    case "wilderness golem":
                        monster = new RockGolemFTM(tile, Game1.player.CombatLevel);
                        break;
                    case "serpent":
                        monster = new SerpentFTM(tile);
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
                        monster = new SquidKidFTM(tile);
                        break;
                    default: break;
                }

                if (monster == null)
                {
                    Monitor.Log($"The monster to be spawned (\"{monsterType.MonsterName}\") doesn't match any known monster types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                    return false;
                }

                monster.MaxHealth = monster.Health; //some monster types set Health on creation and expect MaxHealth to be updated elsewhere

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