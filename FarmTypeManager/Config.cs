using System.Collections.Generic;

namespace FarmTypeManager
{
    //config.json, used by all players/saves for shared functions
    class ModConfig
    {
        public bool EnableWhereAmICommand { get; set; }

        public ModConfig()
        {
            EnableWhereAmICommand = true; //should enable the "whereami" command in the SMAPI console
        }
    }

    //per-character configuration file, e.g. PlayerName12345.json; contains most of the mod's functional settings, split up this way to allow for different settings between saves & farm types
    class FarmConfig
    {
        public bool ForageSpawn_Enabled { get; set; }
        public bool OreSpawn_Enabled { get; set; }

        public ForageGeneration ForageSpawn { get; set; }
        public OreGeneration OreSpawn { get; set; }

        public FarmConfig()
        {
            //basic on/off toggles
            ForageSpawn_Enabled = false;
            OreSpawn_Enabled = false;

            //configure default forage generation settings
            ForageSpawn = new ForageGeneration();
            ForageSpawn.PercentExtraItemsPerForagingLevel = 10; //multiplier to give extra forage per level of foraging skill; default is +10% per level, which means twice as much forage at level 10
            //a set of "SpawnArea" objects, describing where and how forage items can spawn on each map
            ForageSpawn.Areas = new SpawnArea[] { new SpawnArea("Farm", 0, 3, new string[] { "Grass", "Diggable" }, new string[0], new string[0]) };
            //the "parentSheetIndex" values for each type of forage item allowed to spawn in each season (the numbers found in ObjectInformation.xnb)
            ForageSpawn.SpringItemIndex = new int[] { 16, 20, 22, 257 };
            ForageSpawn.SummerItemIndex = new int[] { 396, 398, 402, 404 };
            ForageSpawn.FallItemIndex = new int[] { 281, 404, 420, 422 };
            ForageSpawn.WinterItemIndex = new int[0];

            //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            ForageSpawn.CustomTileIndex = new int[0];

            //configure default ore generation settings
            OreSpawn = new OreGeneration();
            OreSpawn.PercentExtraOrePerMiningLevel = 10; //multiplier to give extra ore per level of mining skill; default is +10% per level, which means twice as much ore at level 10
            //a set of "SpawnArea" objects, describing where ore can spawn on each map
            OreSpawn.Areas = new OreSpawnArea[] { new OreSpawnArea("Farm", 1, 5, new string[] { "Quarry" }, new string[0], new string[0], null, null, null) };
            //mining skill level required to spawn each ore type; defaults are based on the vanilla "hilltop" map settings, though some types didn't spawn at all
            OreSpawn.MiningSkillRequired = new Dictionary<string,  int>();
            OreSpawn.MiningSkillRequired.Add("Stone", 0);
            OreSpawn.MiningSkillRequired.Add("Geode", 0);
            OreSpawn.MiningSkillRequired.Add("FrozenGeode", 5);
            OreSpawn.MiningSkillRequired.Add("MagmaGeode", 8);
            OreSpawn.MiningSkillRequired.Add("Gem", 6);
            OreSpawn.MiningSkillRequired.Add("Copper", 0);
            OreSpawn.MiningSkillRequired.Add("Iron", 4);
            OreSpawn.MiningSkillRequired.Add("Gold", 7);
            OreSpawn.MiningSkillRequired.Add("Iridium", 9);
            OreSpawn.MiningSkillRequired.Add("Mystic", 10);
            //weighted chance to spawn ore at the minimum required skill level (e.g. by default, iron starts spawning at level 4 mining skill with a 15% chance, but is 0% before that)
            OreSpawn.StartingChance = new Dictionary<string, int>();
            OreSpawn.StartingChance.Add("Stone", 66);
            OreSpawn.StartingChance.Add("Geode", 8);
            OreSpawn.StartingChance.Add("FrozenGeode", 4);
            OreSpawn.StartingChance.Add("MagmaGeode", 2);
            OreSpawn.StartingChance.Add("Gem", 5);
            OreSpawn.StartingChance.Add("Copper", 21);
            OreSpawn.StartingChance.Add("Iron", 15);
            OreSpawn.StartingChance.Add("Gold", 10);
            OreSpawn.StartingChance.Add("Iridium", 1);
            OreSpawn.StartingChance.Add("Mystic", 1);
            //weighted chance to spawn ore at level 10 mining skill; for any levels in between "starting" and level 10, the odds are gradually adjusted (e.g. by default, stone is 66% at level 0, 57% at level 5, and 48% at level 10)
            OreSpawn.LevelTenChance = new Dictionary<string, int>();
            OreSpawn.LevelTenChance.Add("Stone", 48);
            OreSpawn.LevelTenChance.Add("Geode", 2);
            OreSpawn.LevelTenChance.Add("FrozenGeode", 2);
            OreSpawn.LevelTenChance.Add("MagmaGeode", 2);
            OreSpawn.LevelTenChance.Add("Gem", 5);
            OreSpawn.LevelTenChance.Add("Copper", 16);
            OreSpawn.LevelTenChance.Add("Iron", 13);
            OreSpawn.LevelTenChance.Add("Gold", 10);
            OreSpawn.LevelTenChance.Add("Iridium", 1);
            OreSpawn.LevelTenChance.Add("Mystic", 1);
            //a list of every tilesheet index commonly used by "quarry" tiles on maps, e.g. the vanilla hilltop (mining) farm
            //these should be compared to [instance of GameLocation].getTileIndexAt(x, y, "Back")
            OreSpawn.QuarryTileIndex = new int[] { 556, 558, 583, 606, 607, 608, 630, 635, 636, 680, 681, 685 };
            //NOTE: swap in the following code to cover more tiles, e.g. the grassy edges of the "quarry" dirt; this tends to cover too much ground and weird spots, though, such as the cave entrance
            //{ 556, 558, 583, 606, 607, 608, 630, 631, 632, 633, 634, 635, 636, 654, 655, 656, 657, 658, 659, 679, 680, 681, 682, 683, 684, 685, 704, 705, 706, 707 };

            //an extra list of tilesheet indices, for use by players who want to make some custom tile detection while still using the "quarry" list
            //DEV NOTE: this could be reworked to allow for infinite lists, but that feels like unnecessary complexity
            OreSpawn.CustomTileIndex = new int[0];
        }

    }

    //contains configuration settings for [re]spawning hardwood sources (large stumps and logs)
    public class HardwoodGeneration
    {
        public SpawnArea[] RespawnAreas { get; set; }
        public SpawnArea[] NewSpawnAreas { get; set; }
    }

    //contains configuration settings for forage item generation behavior
    public class ForageGeneration
    {
        //see FarmConfig's constructor for descriptions of what these do & their default values (and/or the included readme)
        public int PercentExtraItemsPerForagingLevel { get; set; }
        public SpawnArea[] Areas { get; set; }

        public int[] SpringItemIndex { get; set; }
        public int[] SummerItemIndex { get; set; }
        public int[] FallItemIndex { get; set; }
        public int[] WinterItemIndex { get; set; }

        public int[] CustomTileIndex { get; set; }

        public ForageGeneration()
        {

        }
    }

    //contains configuration settings for ore generation behavior
    public class OreGeneration
    {
        //see FarmConfig's constructor for descriptions of what these do & their default values (and/or the included readme)
        public int PercentExtraOrePerMiningLevel { get; set; }
        public OreSpawnArea[] Areas { get; set; }
        public Dictionary<string, int> MiningSkillRequired { get; set; }
        public Dictionary<string, int> StartingChance { get; set; }
        public Dictionary<string, int> LevelTenChance { get; set; }

        public int[] QuarryTileIndex { get; set; }
        public int[] CustomTileIndex { get; set; }

        public OreGeneration()
        {
            
        }
    }

    //a set of variables including a map name, terrain type(s) to auto-detect, and manually defined included/excluded areas for object spawning
    public class SpawnArea
    {
        public string MapName { get; set; }
        public int MinimumSpawnsPerDay { get; set; }
        public int MaximumSpawnsPerDay { get; set; }
        public string[] AutoSpawnTerrainTypes { get; set; } //Valid properties include "Quarry", "Custom", "Diggable", and any tile [Type] properties ("Grass", "Dirt", "Stone", "Wood")
        public string[] IncludeSpawnAreas { get; set; }
        public string[] ExcludeSpawnAreas { get; set; }

        public SpawnArea()
        {

        }

        public SpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude)
        {
            MapName = name;
            MinimumSpawnsPerDay = min;
            MaximumSpawnsPerDay = max;
            AutoSpawnTerrainTypes = types;
            IncludeSpawnAreas = include;
            ExcludeSpawnAreas = exclude;
        }
    }

    //a subclass of "SpawnArea" specifically for ore generation purposes, including additional sets of requirements & spawn chances for ore
    public class OreSpawnArea : SpawnArea
    {
        public Dictionary<string, int> SkillRequired { get; set; }
        public Dictionary<string, int> StartingSpawnChance { get; set; }
        public Dictionary<string, int> LevelTenSpawnChance { get; set; }

        public OreSpawnArea()
            : base ()
        {

        }

        public OreSpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude, Dictionary<string, int> skill, Dictionary<string, int> starting, Dictionary<string, int> levelTen)
            : base (name, min, max, types, include, exclude) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
        {
            SkillRequired = skill;
            StartingSpawnChance = starting;
            LevelTenSpawnChance = levelTen;
        }
    }
}
