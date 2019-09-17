using System;
using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for monster generation
        private class MonsterSpawnArea : SpawnArea
        {
            public List<MonsterType> MonsterTypes { get; set; } //a list of MonsterType objects (each containing a name and optional dictionary of customization settings)

            //default constructor, providing Wilderness Farm style monster spawns on the farm
            public MonsterSpawnArea()
                : base()
            {
                UniqueAreaID = "";
                MapName = "Farm";
                MinimumSpawnsPerDay = 5; //~1/8 chance of 1 spawn each 10m
                MaximumSpawnsPerDay = 16; //~3/8 chance of 1 spawn each 10m
                AutoSpawnTerrainTypes = new string[] { "All" };
                IncludeAreas = new string[0];
                ExcludeAreas = new string[0];
                StrictTileChecking = "High";
                SpawnTiming = new SpawnTiming(1900, 2600, 10, 1, false); //from 5pm onward, 1 monster can spawn every 10m
                ExtraConditions = new ExtraConditions();

                MonsterTypes = new List<MonsterType>();

                /*
                //TODO: wilderness farm default monster types
                Dictionary<string, object> slime1 = new Dictionary<string, object>();
                Dictionary<string, object> slime2 = new Dictionary<string, object>();
                Dictionary<string, object> slime3 = new Dictionary<string, object>();
                (etc)
                */
            }
        }

        /// <summary>A container for a monster's name and a set of optional customization settings.</summary>
        private class MonsterType
        {
            public string MonsterName { get; set; } = ""; //a string representing a specific monster type

            private Dictionary<string, object> settings = null; //a dictionary of optional setting names and values (of various types)
            public Dictionary<string, object> Settings
            {
                get
                {
                    return settings;
                }
                set
                {
                    if (value != null && value.Comparer != StringComparer.OrdinalIgnoreCase) //if the provided dictionary exists & isn't using a case-insensitive comparer
                    {
                        //create and use a copy with a case-insensitive comparer
                        settings = new Dictionary<string, object>(value, StringComparer.OrdinalIgnoreCase);
                    }
                }
            }

            public MonsterType(string monsterName, Dictionary<string, object> settings)
            {
                MonsterName = monsterName;
                Settings = settings;
            }
        }
    }
}