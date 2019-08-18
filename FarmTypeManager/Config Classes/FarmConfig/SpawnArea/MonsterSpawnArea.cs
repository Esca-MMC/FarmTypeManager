using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for monster generation
        private class MonsterSpawnArea : SpawnArea
        {
            public int SpawnStartTime { get; set; } = 1900;
            public int SpawnEndTime { get; set; } = 9999; //TODO: figure out when the day actually sends and set it here

            public int MinimumSecondsBetweenSpawns { get; set; } = 7;
            public int MaximumSecondsBetweenSpawns { get; set; } = 42;

            public int MinimumSimultaneousSpawns { get; set; } = 1;
            public int MaximumSimultaneousSpawns { get; set; } = 1;

            //TODO: monster type to spawn and various per-monster settings (probably its own class)

            //TODO: actually make all these settings match the Wilderness Farm (both the above & below settings)
            //farm monsters seem to start spawning at 7pm, with a 25% chance to spawn (lowered(?) by luck) every 10 minutes
            //have yet to look into type/stats/etc there

            //default constructor, providing Wilderness Farm style monster spawns on the farm
            public MonsterSpawnArea()
                : base()
            {
                UniqueAreaID = "";
                MapName = "Farm";
                MinimumSpawnsPerDay = 999;
                MaximumSpawnsPerDay = 999;
                AutoSpawnTerrainTypes = new string[] { "Any" };
                IncludeAreas = new string[0];
                ExcludeAreas = new string[0];
                StrictTileChecking = "High";
                ExtraConditions = new ExtraConditions();
            }
        }
    }
}