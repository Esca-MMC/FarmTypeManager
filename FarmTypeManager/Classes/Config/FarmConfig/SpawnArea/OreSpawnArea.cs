﻿using StardewModdingAPI;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for ore generation, providing the ability to optionally override each area's skill requirements & spawn chances for ore
        private class OreSpawnArea : SpawnArea
        {
            public Dictionary<string, int> MiningLevelRequired { get; set; } = null;
            public Dictionary<string, int> StartingSpawnChance { get; set; } = null;
            public Dictionary<string, int> LevelTenSpawnChance { get; set; } = null;

            //default constructor, providing Hilltop Farm style ore on the farm's quarry terrain (if any)
            public OreSpawnArea()
                : base()
            {
                MapName = "Farm";
                MinimumSpawnsPerDay = 1;
                MaximumSpawnsPerDay = 5;
                IncludeTerrainTypes = new string[] { "Quarry" };
            }
        }
    }
}