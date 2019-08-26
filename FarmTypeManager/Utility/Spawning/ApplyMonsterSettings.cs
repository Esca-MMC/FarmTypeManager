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
            /// <summary>Apply any provided non-type-specific settings to a monster.</summary>
            /// <param name="monster">The monster to be customized. This will be modified by reference.</param>
            /// <param name="settings">A dictionary of setting names and values. If null, this method will do nothing.</param>
            /// <param name="area">The monster's SpawnArea. Required for log messages.</param>
            public static void ApplyMonsterSettings(Monster monster, Dictionary<string, object> settings, SpawnArea area)
            {
                if (settings == null) //if no settings were provided
                {
                    return; //do nothing
                }

                //set max HP
                if (settings.ContainsKey("HP"))
                {
                    monster.MaxHealth = Convert.ToInt32(settings["HP"]);
                    monster.Health = monster.MaxHealth; //set current HP to max
                }

                //set damage
                if (settings.ContainsKey("Damage"))
                {
                    monster.DamageToFarmer = Convert.ToInt32(settings["Damage"]);
                }

                //multiply HP and/or damage based on players' highest level in a skill
                if (settings.ContainsKey("RelatedSkill"))
                {
                    //parse the provided skill into an enum
                    Utility.Skills skill = (Utility.Skills)Enum.Parse(typeof(Utility.Skills), (string)settings["RelatedSkill"], true);

                    //multiply HP
                    if (settings.ContainsKey("PercentExtraHPPerSkillLevel"))
                    {
                        //calculate HP multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraHPPerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's max HP
                        skillMultiplier *= monster.MaxHealth; //multiply the current max HP by the skill multiplier
                        monster.MaxHealth = (int)skillMultiplier; //set the monster's new max HP (rounded down to the nearest integer)
                        monster.Health = monster.MaxHealth; //set current HP to max
                    }

                    //multiply damage
                    if (settings.ContainsKey("PercentExtraDamagePerSkillLevel"))
                    {
                        //calculate damage multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraDamagePerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's damage
                        skillMultiplier *= monster.DamageToFarmer; //multiply the current damage by the skill multiplier
                        monster.DamageToFarmer = (int)skillMultiplier; //set the monster's new damage (rounded down to the nearest integer)
                    }
                }

                //set loot (i.e. items dropped on death by the monster)
                if (settings.ContainsKey("Loot"))
                {
                    List<int> IDs = GetIDsFromObjects((List<object>)settings["Loot"], area); //parse the provided setting into a list of object IDs

                    monster.objectsToDrop.Clear(); //clear any existing loot
                    foreach (int ID in IDs) //for each object ID
                    {
                        monster.objectsToDrop.Add(ID); //add it to the loot list
                    }
                }
            }
        }
    }
}