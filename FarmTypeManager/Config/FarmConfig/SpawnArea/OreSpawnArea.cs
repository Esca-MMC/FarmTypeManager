using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for ore generation, providing the ability to optionally override each area's skill requirements & spawn chances for ore
        private class OreSpawnArea : SpawnArea
        {
            public Dictionary<string, int> MiningLevelRequired { get; set; }
            public Dictionary<string, int> StartingSpawnChance { get; set; }
            public Dictionary<string, int> LevelTenSpawnChance { get; set; }

            public OreSpawnArea()
                : base()
            {

            }

            public OreSpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra, Dictionary<string, int> skill, Dictionary<string, int> starting, Dictionary<string, int> levelTen)
                : base(id, name, min, max, types, include, exclude, safety, extra) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
            {
                MiningLevelRequired = skill;
                StartingSpawnChance = starting;
                LevelTenSpawnChance = levelTen;
            }
        }
    }
}