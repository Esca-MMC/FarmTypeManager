using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A group of settings that defines when an area's spawns will appear.</summary>
        private class SpawnTiming
        {
            public int StartTime { get; set; } = 600; //the stardew time value when spawning should start (inclusive)
            public int EndTime { get; set; } = 600; //the stardew time value when spawning should end (inclusive)

            private int? minimumTimeBetweenSpawns = 10; //the minimum amount of in-game time (in 10-minute increments) between each "set" of spawns (note: this may override min/max spawns per day from SpawnArea)
            public int? MinimumTimeBetweenSpawns
            {
                get
                {
                    return minimumTimeBetweenSpawns;
                }
                set
                {
                    if (minimumTimeBetweenSpawns == null || minimumTimeBetweenSpawns < 10) //if this is null or less than 10
                    {
                        minimumTimeBetweenSpawns = 10; //set it to 10
                    }

                    if (minimumTimeBetweenSpawns % 10 > 0) //if this isn't a multiple of 10
                    {
                        minimumTimeBetweenSpawns = (minimumTimeBetweenSpawns - (minimumTimeBetweenSpawns % 10)) + 10; //round up to a multiple of 10
                    }

                    minimumTimeBetweenSpawns = value;
                }
            }

            private int? MaximumSimultaneousSpawns { get; set; } = 1; //the maximum number of things this area will spawn at the same time (note: this may override min/max spawns per day from SpawnArea)

            public bool OnlySpawnIfAPlayerIsPresent = false; //if true, spawns will be skipped if no players are present at the target map

            public SpawnTiming()
            {

            }

            public SpawnTiming(int start, int end, int between, int simultaneous, bool present)
            {
                StartTime = start;
                EndTime = end;
                MinimumTimeBetweenSpawns = between;
                MaximumSimultaneousSpawns = simultaneous;
                OnlySpawnIfAPlayerIsPresent = present;
            }
        }
    }
}