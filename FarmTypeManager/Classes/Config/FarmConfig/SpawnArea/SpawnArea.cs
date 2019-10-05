using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A set of variables describing the spawn settings for an "area" on a single map. Used primarily by subclasses in most versions.</summary>
        private class SpawnArea
        {
            public string UniqueAreaID { get; set; } = "";
            public string MapName { get; set; } = "Farm";
            public int MinimumSpawnsPerDay { get; set; } = 0;
            public int MaximumSpawnsPerDay { get; set; } = 0;

            public string[] AutoSpawnTerrainTypes //supports a previously used name for IncludeTerrainTypes (changed in v1.7.0)
            {
                set
                {
                    IncludeTerrainTypes = value;
                }
            }

            private string[] includeTerrainTypes = new string[0];
            public string[] IncludeTerrainTypes
            {
                get
                {
                    return includeTerrainTypes ?? new string[0]; //return default if null
                }
                set
                {
                    includeTerrainTypes = value;
                }
            }

            private string[] includeAreas = new string[0];
            public string[] IncludeAreas
            {
                get
                {
                    return includeAreas ?? new string[0]; //return default if null
                }
                set
                {
                    includeAreas = value;
                }
            }

            private string[] excludeAreas = new string[0];
            public string[] ExcludeAreas
            {
                get
                {
                    return excludeAreas ?? new string[0]; //return default if null
                }
                set
                {
                    excludeAreas = value;
                }
            }

            public string StrictTileChecking { get; set; } = "High";

            private SpawnTiming spawnTiming;
            public SpawnTiming SpawnTiming
            {
                get
                {
                    return spawnTiming ?? new SpawnTiming(); //return default if null
                }
                set
                {
                    spawnTiming = value;
                }
            }

            private ExtraConditions extraConditions = new ExtraConditions();
            public ExtraConditions ExtraConditions
            {
                get
                {
                    return extraConditions ?? new ExtraConditions(); //return default if null
                }
                set
                {
                    extraConditions = value;
                }
            }

            public int? DaysUntilSpawnsExpire { get; set; } = null;

            public SpawnArea()
            {

            }
        }
    }
}