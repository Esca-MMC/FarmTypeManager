using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing any per-farm information that needs to be saved by the mod. Not normally intended to be edited by the player.</summary>
        private class InternalSaveData
        {
            //class added in version 1.3; defaults used here to automatically fill in values with SMAPI's json interface
            //note that as of version 1.4, this is being moved from within FarmConfig to its own json file

            public Utility.Weather WeatherForYesterday { get; set; } = Utility.Weather.Sunny;
            public Dictionary<string, int> LNOSCounter { get; set; } = new Dictionary<string, int>(); //added in version 1.4
            public bool ExistingObjectsFound { get; set; } = false; //added in version 1.4.1
            public Dictionary<string, string[]> ExistingObjectLocations { get; set; } = new Dictionary<string, string[]>(); //added in version 1.4.1

            public InternalSaveData()
            {

            }

            public InternalSaveData(Utility.Weather wyesterday, Dictionary<string, int> counter, Dictionary<string, string[]> locations)
            {
                WeatherForYesterday = wyesterday; //an enum (int) value corresponding to yesterday's weather
                LNOSCounter = counter; //dictionary for LimitedNumberOfSpawns tracking; designed to use SpawnArea.UniqueAreaID as a key, and increment the value each day items spawn in an area
                ExistingObjectsFound = false; //indicates whether FindExistingObjectLocations has already been performed
                ExistingObjectLocations = locations; //dictionary of IncludeArea coordinates, filled by FindExistingObjectLocations; designed to use SpawnArea.UniqueAreaID as a key
            }
        }
    }
}