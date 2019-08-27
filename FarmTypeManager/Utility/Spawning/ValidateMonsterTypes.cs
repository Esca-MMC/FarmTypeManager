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
using Newtonsoft.Json.Linq;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Removes any invalid monster types and/or settings from a list.</summary>
            /// <param name="monsterTypes">A list of monster type data.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A new list of MonsterTypes with any invalid monster types and/or settings removed.</returns>
            public static List<MonsterType> ValidateMonsterTypes(List<MonsterType> monsterTypes, string areaID = "")
            {
                if (monsterTypes == null || monsterTypes.Count <= 0) //if the provided list is null or empty
                {
                    return new List<MonsterType>(); //return an empty list
                }

                List<MonsterType> validTypes = new List<MonsterType>(monsterTypes); //a new copy of the list, to be validated and returned
                List<int> indicesToDelete = new List<int>(); //a list of indices in the valid monster list to be deleted (e.g. due to invalid names)

                for (int x = 0; x < validTypes.Count; x++) //for each monster type in the new list
                {
                    //validate monster names
                    bool validName = false;

                    //NOTE: switch cases copied from SpawnMonster.cs; update this if new monsters are added
                    switch (validTypes[x].MonsterName.ToLower()) //avoid any casing issues by making this lower-case
                    {
                        case "bat":
                        case "bigslime":
                        case "big slime":
                        case "bug":
                        case "duggy":
                        case "dust":
                        case "spirit":
                        case "dustspirit":
                        case "dust spirit":
                        case "fly":
                        case "ghost":
                        case "slime":
                        case "greenslime":
                        case "green slime":
                        case "grub":
                        case "iridiumcrab":
                        case "iridium crab":
                        case "lavacrab":
                        case "lava crab":
                        case "metalhead":
                        case "metal head":
                        case "mummy":
                        case "rockcrab":
                        case "rock crab":
                        case "stonegolem":
                        case "stone golem":
                        case "serpent":
                        case "brute":
                        case "shadowbrute":
                        case "shadow brute":
                        case "shaman":
                        case "shadowshaman":
                        case "shadow shaman":
                        case "skeleton":
                        case "squid":
                        case "kid":
                        case "squidkid":
                        case "squid kid":
                        case "wildernessgolem":
                        case "wilderness golem":
                            validName = true; //the name is valid
                            break;
                        default: break; //the name is invalid
                    }

                    if (!validName) //if the name is invalid
                    {
                        Monitor.Log($"A listed monster (\"{validTypes[x].MonsterName}\") doesn't match any known monster types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                        indicesToDelete.Add(x); //add this type's index to the list for deletion
                        continue; //skip to the next monster type
                    }

                    //validate HP
                    if (validTypes[x].Settings.ContainsKey("HP"))
                    {
                        if (validTypes[x].Settings["HP"] is long) //if this is a readable integer
                        {
                            int HP = Convert.ToInt32(validTypes[x].Settings["HP"]);
                            if (HP <= 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"HP\" setting for monster type \"{validTypes[x].MonsterName}\" is {HP}. Setting it to 1.", LogLevel.Trace);
                                monsterTypes[x].Settings["HP"] = (long)1; //set the original provided setting to 1
                                validTypes[x].Settings["HP"] = (long)1; //set the validated setting to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"HP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("HP"); //remove the setting
                        }
                    }

                    //validate damage
                    if (validTypes[x].Settings.ContainsKey("Damage"))
                    {
                        if (validTypes[x].Settings["Damage"] is long) //if this is a readable integer
                        {
                            int damage = Convert.ToInt32(validTypes[x].Settings["Damage"]);
                            if (damage < 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"Damage\" setting for monster type \"{validTypes[x].MonsterName}\" is {damage}. Setting it to 0.", LogLevel.Trace);
                                monsterTypes[x].Settings["Damage"] = (long)0; //set the original provided setting to 0
                                validTypes[x].Settings["Damage"] = (long)0; //set the validated setting to 0
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"Damage\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Damage"); //remove the setting
                        }
                    }

                    //validate movement speed
                    if (validTypes[x].Settings.ContainsKey("Speed"))
                    {
                        if (validTypes[x].Settings["Speed"] is long) //if this is a readable integer
                        {
                            int speed = Convert.ToInt32(validTypes[x].Settings["Speed"]);
                            if (speed < 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"Speed\" setting for monster type \"{validTypes[x].MonsterName}\" is {speed}. Setting it to 0.", LogLevel.Trace);
                                monsterTypes[x].Settings["Speed"] = (long)0; //set the original provided setting to 0
                                validTypes[x].Settings["Speed"] = (long)0; //set the validated setting to 0
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"Speed\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Speed"); //remove the setting
                        }
                    }

                    //validate experience points
                    if (validTypes[x].Settings.ContainsKey("EXP"))
                    {
                        if (validTypes[x].Settings["EXP"] is long) //if this is a readable integer
                        {
                            int exp = Convert.ToInt32(validTypes[x].Settings["EXP"]);
                            if (exp < 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"EXP\" setting for monster type \"{validTypes[x].MonsterName}\" is {exp}. Setting it to 0.", LogLevel.Trace);
                                monsterTypes[x].Settings["EXP"] = (long)0; //set the original provided setting to 0
                                validTypes[x].Settings["EXP"] = (long)0; //set the validated setting to 0
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"EXP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("EXP"); //remove the setting
                        }
                    }

                    //validate the related skill setting
                    if (validTypes[x].Settings.ContainsKey("RelatedSkill"))
                    {
                        if (validTypes[x].Settings["RelatedSkill"] is string) //if this is a readable string
                        {
                            string relatedSkill = ((string)validTypes[x].Settings["RelatedSkill"]).Trim().ToLower(); //parse the provided skill, trim whitespace, and lower case
                            bool isSkill = false;

                            foreach (string skill in Enum.GetNames(typeof(Skills))) //for each known in-game skill
                            {
                                if (relatedSkill.Trim().ToLower() == skill.Trim().ToLower()) //if the provided skill name matches this known skill
                                {
                                    isSkill = true; //the provided skill is valid
                                }
                            }

                            if (!isSkill) //if this isn't a known skill
                            {
                                Monitor.Log($"The \"RelatedSkill\" setting for monster type \"{validTypes[x].MonsterName}\" doesn't seem to be a known skill. Please make sure it's spelled correctly.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                                validTypes[x].Settings.Remove("RelatedSkill"); //remove the setting
                            }
                        }
                        else //if this isn't a readable string
                        {
                            Monitor.Log($"The \"RelatedSkill\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's a valid string (text inside quotation marks).", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("RelatedSkill"); //remove the setting
                        }
                    }

                    //validate HP multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraHPPerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraHPPerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraHPPerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraHPPerSkillLevel"); //remove the setting
                        }

                    }

                    //validate damage multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraDamagePerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraDamagePerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraDamagePerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraDamagePerSkillLevel"); //remove the setting
                        }
                    }

                    //validate speed multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraSpeedPerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraSpeedPerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraSpeedPerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraSpeedPerSkillLevel"); //remove the setting
                        }

                    }

                    //validate experience multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraEXPPerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraEXPPerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraEXPPerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraEXPPerSkillLevel"); //remove the setting
                        }
                    }

                    //validate loot and parse the provided list into IDs
                    if (validTypes[x].Settings.ContainsKey("Loot"))
                    {
                        List<object> rawList = null;

                        try
                        {
                            rawList = ((JArray)validTypes[x].Settings["Loot"]).ToObject<List<object>>(); //cast this list to catch formatting/coding errors
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"The \"Loot\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's a correctly formatted list.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Loot"); //remove the setting
                        }

                        if (validTypes[x].Settings.ContainsKey("Loot")) //if no exception happened
                        {
                            if (rawList == null) //if a null list was provided
                            {
                                validTypes[x].Settings["Loot"] = new List<int>(); //use an empty list of IDs
                            }
                            else //if an actual list was provided
                            {
                                validTypes[x].Settings["Loot"] = GetIDsFromObjects(rawList, areaID); //parse the list into valid IDs
                            }
                        }
                    }

                    //validate current HP
                    if (validTypes[x].Settings.ContainsKey("CurrentHP"))
                    {
                        if (validTypes[x].Settings["CurrentHP"] is long) //if this is a readable integer
                        {
                            int currentHP = Convert.ToInt32(validTypes[x].Settings["CurrentHP"]);
                            if (currentHP <= 0) //if the current HP setting is too low
                            {
                                Monitor.Log($"The \"CurrentHP\" setting for monster type \"{validTypes[x].MonsterName}\" is {currentHP}. Setting it to 1.", LogLevel.Trace);
                                monsterTypes[x].Settings["CurrentHP"] = (long)1; //set the original provided setting to 1
                                validTypes[x].Settings["CurrentHP"] = (long)1; //set the validated setting to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"CurrentHP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("CurrentHP"); //remove the setting
                        }
                    }
                }

                foreach (int x in indicesToDelete) //for each index of a monster type to be deleted
                {
                    validTypes.RemoveAt(x); //remove it
                }

                return validTypes;
            }
        }
    }
}