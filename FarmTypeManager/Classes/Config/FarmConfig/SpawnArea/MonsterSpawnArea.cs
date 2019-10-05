using System;
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
                IncludeTerrainTypes = new string[] { "All" };
                IncludeAreas = new string[0];
                ExcludeAreas = new string[0];
                StrictTileChecking = "High";
                SpawnTiming = new SpawnTiming
                {
                    //from 5pm onward, up to 1 monster can spawn every 10 minutes
                    StartTime = 1900,
                    EndTime = 2600,
                    MinimumTimeBetweenSpawns = 10,
                    MaximumSimultaneousSpawns = 1,
                    OnlySpawnIfAPlayerIsPresent = false,
                    SpawnSound = ""
                }; 
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
    }
}