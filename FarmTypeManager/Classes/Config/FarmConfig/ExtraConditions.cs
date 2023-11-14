using StardewModdingAPI;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A set of additional requirements needed to spawn objects in a given area.</summary>
        private class ExtraConditions
        {
            public string[] Years { get; set; } = new string[0];
            public string[] Seasons { get; set; } = new string[0];
            public string[] Days { get; set; } = new string[0];
            public string[] WeatherYesterday { get; set; } = new string[0];
            public string[] WeatherToday { get; set; } = new string[0];
            public string[] WeatherTomorrow { get; set; } = new string[0];
            public string[] GameStateQueries { get; set; } = new string[0];
            public Dictionary<string, string> CPConditions { get; set; } = new();
            public string[] EPUPreconditions { get; set; } = new string[0];
            public int? LimitedNumberOfSpawns { get; set; } = null;

            public ExtraConditions()
            {

            }
        }
    }
}