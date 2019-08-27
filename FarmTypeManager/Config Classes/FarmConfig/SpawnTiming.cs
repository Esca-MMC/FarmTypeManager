using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A group of settings that defines when an area's spawns will appear.</summary>
        private class SpawnTiming
        {
            private int? startTime = 600; //the stardew time value when spawning should start (inclusive)
            public int? StartTime
            {
                get
                {
                    if (startTime > endTime) //if start and end are in the wrong order (and neither is null)
                    {
                        //swap start and end
                        int? temp = startTime;
                        startTime = endTime;
                        endTime = temp;
                    }

                    return startTime;
                }

                set
                {
                    startTime = value;

                    if (startTime == null || startTime < 10) //if this is null or less than 10
                    {
                        startTime = 10; //set it to 10
                    }
                    else if (startTime % 10 > 0) //if this isn't a multiple of 10
                    {
                        startTime = (startTime - (startTime % 10)) + 10; //round up to a multiple of 10
                    }
                }
            }

            private int? endTime = 600; //the stardew time value when spawning should end (inclusive)
            public int? EndTime
            {
                get
                {
                    if (startTime > endTime) //if start and end are in the wrong order (and neither is null)
                    {
                        //swap start and end
                        int? temp = startTime;
                        startTime = endTime;
                        endTime = temp;
                    }

                    return endTime;
                }

                set
                {
                    endTime = value;

                    if (endTime == null || endTime < 10) //if this is null or less than 10
                    {
                        endTime = 10; //set it to 10
                    }
                    else if (endTime % 10 > 0) //if this isn't a multiple of 10
                    {
                        endTime = (endTime - (endTime % 10)) + 10; //round up to a multiple of 10
                    }
                }
            }

            private int? minimumTimeBetweenSpawns = 10; //the minimum amount of in-game time (in 10-minute increments) between each "set" of spawns (note: this may override min/max spawns per day from SpawnArea)
            public int? MinimumTimeBetweenSpawns
            {
                get
                {
                    return minimumTimeBetweenSpawns;
                }
                set
                {
                    minimumTimeBetweenSpawns = value;

                    if (minimumTimeBetweenSpawns == null || minimumTimeBetweenSpawns < 10) //if this is null or less than 10
                    {
                        minimumTimeBetweenSpawns = 10; //set it to 10
                    }
                    else if (minimumTimeBetweenSpawns % 10 > 0) //if this isn't a multiple of 10
                    {
                        minimumTimeBetweenSpawns = (minimumTimeBetweenSpawns - (minimumTimeBetweenSpawns % 10)) + 10; //round up to a multiple of 10
                    }
                }
            }

            private int? MaximumSimultaneousSpawns { get; set; } = null; //the maximum number of things this area will spawn at the same time (note: this may override min/max spawns per day from SpawnArea)

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