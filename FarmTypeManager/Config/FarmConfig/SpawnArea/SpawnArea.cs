using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a set of variables including a map name, terrain type(s) to auto-detect, and manually defined included/excluded areas for object spawning
        private class SpawnArea
        {
            public string UniqueAreaID { get; set; } = ""; //added in version 1.4
            public string MapName { get; set; }
            private int minimumSpawnsPerDay;
            private int maximumSpawnsPerDay;
            private string[] autoSpawnTerrainTypes; //Valid properties include "Quarry", "Custom", "Diggable", "All", and any tile Type properties ("Grass", "Dirt", "Stone", "Wood", etc)
            private string[] includeAreas;
            private string[] excludeAreas;
            public string StrictTileChecking { get; set; } = "High"; //added in version 1.1; default used here to automatically fill it in with SMAPI's json interface
            private ExtraConditions extraConditions; //added in version 1.3

            //custom properties added in version 1.5.0 to handle configuration errors
            public int MinimumSpawnsPerDay
            {
                get
                {
                    if (minimumSpawnsPerDay > maximumSpawnsPerDay) //if the min and max are in the wrong order
                    {
                        //swap min and max
                        int temp = minimumSpawnsPerDay;
                        minimumSpawnsPerDay = maximumSpawnsPerDay;
                        maximumSpawnsPerDay = temp;
                    }

                    return minimumSpawnsPerDay;
                }
                set
                {
                    minimumSpawnsPerDay = value;
                }
            }
            public int MaximumSpawnsPerDay
            {
                get
                {
                    if (minimumSpawnsPerDay > maximumSpawnsPerDay) //if the min and max are in the wrong order
                    {
                        //swap min and max
                        int temp = minimumSpawnsPerDay;
                        minimumSpawnsPerDay = maximumSpawnsPerDay;
                        maximumSpawnsPerDay = temp;
                    }

                    return maximumSpawnsPerDay;
                }
                set
                {
                    maximumSpawnsPerDay = value;
                }
            }

            public string[] AutoSpawnTerrainTypes
            {
                get
                {
                    return autoSpawnTerrainTypes ?? new string[0]; //return default if null
                }
                set
                {
                    autoSpawnTerrainTypes = value;
                }
            }

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

            public SpawnArea()
            {

            }

            public SpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra)
            {
                UniqueAreaID = id; //a unique name assigned to this SpawnArea; used by the save data system
                MapName = name; //a name of the targeted map, e.g. "Farm" or "BusStop"
                MinimumSpawnsPerDay = min; //minimum number of items to be spawned each day (before multipliers)
                MaximumSpawnsPerDay = max; //maximum number of items to be spawned each day (before multipliers)
                AutoSpawnTerrainTypes = types; //a list of strings describing the terrain on which objects may spawn
                IncludeAreas = include; //a list of strings describing coordinates for object spawning
                ExcludeAreas = exclude; //a list of strings describing coordinates *preventing* object spawning
                StrictTileChecking = safety; //the degree of safety-checking to use before spawning objects on a tile
                ExtraConditions = extra; //a list of additional conditions that may be used to limit object spawning
            }
        }
    }
}