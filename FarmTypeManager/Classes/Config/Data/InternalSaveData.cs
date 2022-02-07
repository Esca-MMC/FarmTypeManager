using StardewModdingAPI;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing any per-farm information that needs to be saved by the mod. Not normally intended to be edited by the player.</summary>
        private class InternalSaveData
        {
            public string WeatherForYesterday { get; set; } = "Sunny";
            public Dictionary<string, int> LNOSCounter { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, string[]> ExistingObjectLocations { get; set; } = new Dictionary<string, string[]>();
            public List<SavedObject> SavedObjects { get; set; } = new List<SavedObject>();

            public InternalSaveData()
            {

            }
        }
    }
}