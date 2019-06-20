using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //contains configuration settings for ore generation behavior
        private class OreSettings
        {

            public OreSpawnArea[] Areas { get; set; }
            public int PercentExtraSpawnsPerMiningLevel { get; set; }
            public Dictionary<string, int> MiningLevelRequired { get; set; }
            public Dictionary<string, int> StartingSpawnChance { get; set; }
            public Dictionary<string, int> LevelTenSpawnChance { get; set; }
            public int[] CustomTileIndex { get; set; }

            //default constructor: configure default ore generation settings
            public OreSettings()
            {

                Areas = new OreSpawnArea[] { new OreSpawnArea("", "Farm", 1, 5, new string[] { "Quarry" }, new string[0], new string[0], "High", new ExtraConditions(), null, null, null) }; //a set of "OreSpawnArea" objects, describing where ore can spawn on each map
                PercentExtraSpawnsPerMiningLevel = 0; //multiplier to give extra ore per level of mining skill; default is +0%, since the native game lacks this mechanic

                //mining skill level required to spawn each ore type; defaults are based on the vanilla "hilltop" map settings, though some types didn't spawn at all
                MiningLevelRequired = new Dictionary<string, int>();
                MiningLevelRequired.Add("Stone", 0);
                MiningLevelRequired.Add("Geode", 0);
                MiningLevelRequired.Add("FrozenGeode", 5);
                MiningLevelRequired.Add("MagmaGeode", 8);
                MiningLevelRequired.Add("Gem", 6);
                MiningLevelRequired.Add("Copper", 0);
                MiningLevelRequired.Add("Iron", 4);
                MiningLevelRequired.Add("Gold", 7);
                MiningLevelRequired.Add("Iridium", 9);
                MiningLevelRequired.Add("Mystic", 10);

                //weighted chance to spawn ore at the minimum required skill level (e.g. by default, iron starts spawning at level 4 mining skill with a 15% chance, but is 0% before that)
                StartingSpawnChance = new Dictionary<string, int>();
                StartingSpawnChance.Add("Stone", 66);
                StartingSpawnChance.Add("Geode", 8);
                StartingSpawnChance.Add("FrozenGeode", 4);
                StartingSpawnChance.Add("MagmaGeode", 2);
                StartingSpawnChance.Add("Gem", 5);
                StartingSpawnChance.Add("Copper", 21);
                StartingSpawnChance.Add("Iron", 15);
                StartingSpawnChance.Add("Gold", 10);
                StartingSpawnChance.Add("Iridium", 1);
                StartingSpawnChance.Add("Mystic", 1);

                //weighted chance to spawn ore at level 10 mining skill; for any levels in between "starting" and level 10, the odds are gradually adjusted (e.g. by default, stone is 66% at level 0, 57% at level 5, and 48% at level 10)
                LevelTenSpawnChance = new Dictionary<string, int>();
                LevelTenSpawnChance.Add("Stone", 48);
                LevelTenSpawnChance.Add("Geode", 2);
                LevelTenSpawnChance.Add("FrozenGeode", 2);
                LevelTenSpawnChance.Add("MagmaGeode", 2);
                LevelTenSpawnChance.Add("Gem", 5);
                LevelTenSpawnChance.Add("Copper", 16);
                LevelTenSpawnChance.Add("Iron", 13);
                LevelTenSpawnChance.Add("Gold", 10);
                LevelTenSpawnChance.Add("Iridium", 1);
                LevelTenSpawnChance.Add("Mystic", 1);

                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            }
        }
    }
}