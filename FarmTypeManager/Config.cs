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
        public bool ForageSpawnEnabled { get; set; }
        public bool HardwoodSpawnEnabled { get; set; }
        public bool OreSpawnEnabled { get; set; }

        public ForageSettings Forage_Spawn_Settings { get; set; }
        public HardwoodSettings Hardwood_Spawn_Settings { get; set; }
        public OreSettings Ore_Spawn_Settings { get; set; }

        public int[] QuarryTileIndex { get; set; }

    public FarmConfig()
        {
            //basic on/off toggles
            ForageSpawnEnabled = false;
            HardwoodSpawnEnabled = false;
            OreSpawnEnabled = false;

            //settings for each generation type (assigned in the constructor for each of these "Settings" objects; see those for details)
            Forage_Spawn_Settings = new ForageSettings();
            Hardwood_Spawn_Settings = new HardwoodSettings();
            Ore_Spawn_Settings = new OreSettings();

            //a list of every tilesheet index commonly used by "quarry" tiles on maps, e.g. the vanilla hilltop (mining) farm
            //these should be compared to [an instance of GameLocation].getTileIndexAt(x, y, "Back")
            QuarryTileIndex = new int[] { 556, 558, 583, 606, 607, 608, 630, 635, 636, 680, 681, 685 };
            //NOTE: swap in the following code to cover more tiles, e.g. the grassy edges of the "quarry" dirt; this tends to cover too much ground and weird spots, though, such as the farm's cave entrance
            //{ 556, 558, 583, 606, 607, 608, 630, 631, 632, 633, 634, 635, 636, 654, 655, 656, 657, 658, 659, 679, 680, 681, 682, 683, 684, 685, 704, 705, 706, 707 };
    }

}

    //contains configuration settings for forage item generation behavior
    public class ForageSettings
    {
        public int PercentExtraItemsPerForagingLevel { get; set; }
        public SpawnArea[] Areas { get; set; }
        public int[] SpringItemIndex { get; set; }
        public int[] SummerItemIndex { get; set; }
        public int[] FallItemIndex { get; set; }
        public int[] WinterItemIndex { get; set; }
        public int[] CustomTileIndex { get; set; }

        //default constructor: configure default forage generation settings
        public ForageSettings()
        {
            PercentExtraItemsPerForagingLevel = 10; //multiplier to give extra forage per level of foraging skill; default is +10% per level, which means twice as much forage at level 10
            Areas = new SpawnArea[] { new SpawnArea("Farm", 0, 3, new string[] { "Grass", "Diggable" }, new string[0], new string[0]) }; //a set of "SpawnArea" objects, describing where forage items can spawn on each map
            //the "parentSheetIndex" values for each type of forage item allowed to spawn in each season (the numbers found in ObjectInformation.xnb)
            SpringItemIndex = new int[] { 16, 20, 22, 257 };
            SummerItemIndex = new int[] { 396, 398, 402, 404 };
            FallItemIndex = new int[] { 281, 404, 420, 422 };
            WinterItemIndex = new int[0];
            //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            CustomTileIndex = new int[0]; 
        }
    }

    //contains configuration settings for [re]spawning hardwood sources (large stumps and logs)
    public class HardwoodSettings
    {
        public int PercentExtraSpawnsPerForageLevel { get; set; }
        public HardwoodSpawnArea[] Areas { get; set; }
        public int[] CustomTileIndex { get; set; }

        public HardwoodSettings()
        {
            PercentExtraSpawnsPerForageLevel = 0; //multiplier to give extra hardwood objects per level of forage skill; default is 0%, but it's included for those who want areas with variable spawn rates
            Areas = new HardwoodSpawnArea[] { new HardwoodSpawnArea("Farm", 999, 999, new string[0], new string[0], new string[0], 1, 0, false) }; //a set of "HardwoodSpawnArea" objects, describing where hardwood objects can spawn on each map
            CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
        }
    }

    //contains configuration settings for ore generation behavior
    public class OreSettings
    {
        public int PercentExtraOrePerMiningLevel { get; set; }
        public OreSpawnArea[] Areas { get; set; }
        public Dictionary<string, int> MiningSkillRequired { get; set; }
        public Dictionary<string, int> StartingChance { get; set; }
        public Dictionary<string, int> LevelTenChance { get; set; }
        public int[] CustomTileIndex { get; set; }

        //default constructor: configure default ore generation settings
        public OreSettings()
        {
            PercentExtraOrePerMiningLevel = 10; //multiplier to give extra ore per level of mining skill; default is +10% per level, which means twice as much ore at level 10
            Areas = new OreSpawnArea[] { new OreSpawnArea("Farm", 1, 5, new string[] { "Quarry" }, new string[0], new string[0], null, null, null) }; //a set of "OreSpawnArea" objects, describing where ore can spawn on each map
            //mining skill level required to spawn each ore type; defaults are based on the vanilla "hilltop" map settings, though some types didn't spawn at all
            MiningSkillRequired = new Dictionary<string, int>();
            MiningSkillRequired.Add("Stone", 0);
            MiningSkillRequired.Add("Geode", 0);
            MiningSkillRequired.Add("FrozenGeode", 5);
            MiningSkillRequired.Add("MagmaGeode", 8);
            MiningSkillRequired.Add("Gem", 6);
            MiningSkillRequired.Add("Copper", 0);
            MiningSkillRequired.Add("Iron", 4);
            MiningSkillRequired.Add("Gold", 7);
            MiningSkillRequired.Add("Iridium", 9);
            MiningSkillRequired.Add("Mystic", 10);
            //weighted chance to spawn ore at the minimum required skill level (e.g. by default, iron starts spawning at level 4 mining skill with a 15% chance, but is 0% before that)
            StartingChance = new Dictionary<string, int>();
            StartingChance.Add("Stone", 66);
            StartingChance.Add("Geode", 8);
            StartingChance.Add("FrozenGeode", 4);
            StartingChance.Add("MagmaGeode", 2);
            StartingChance.Add("Gem", 5);
            StartingChance.Add("Copper", 21);
            StartingChance.Add("Iron", 15);
            StartingChance.Add("Gold", 10);
            StartingChance.Add("Iridium", 1);
            StartingChance.Add("Mystic", 1);
            //weighted chance to spawn ore at level 10 mining skill; for any levels in between "starting" and level 10, the odds are gradually adjusted (e.g. by default, stone is 66% at level 0, 57% at level 5, and 48% at level 10)
            LevelTenChance = new Dictionary<string, int>();
            LevelTenChance.Add("Stone", 48);
            LevelTenChance.Add("Geode", 2);
            LevelTenChance.Add("FrozenGeode", 2);
            LevelTenChance.Add("MagmaGeode", 2);
            LevelTenChance.Add("Gem", 5);
            LevelTenChance.Add("Copper", 16);
            LevelTenChance.Add("Iron", 13);
            LevelTenChance.Add("Gold", 10);
            LevelTenChance.Add("Iridium", 1);
            LevelTenChance.Add("Mystic", 1);
            //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            //DEVELOPMENT NOTE: this could be reworked to allow for infinite lists, but that feels like unnecessary complexity
            CustomTileIndex = new int[0];
        }
    }

    //a set of variables including a map name, terrain type(s) to auto-detect, and manually defined included/excluded areas for object spawning
    public class SpawnArea
    {
        public string MapName { get; set; }
        public int MinimumSpawnsPerDay { get; set; }
        public int MaximumSpawnsPerDay { get; set; }
        public string[] AutoSpawnTerrainTypes { get; set; } //Valid properties include "Quarry", "Custom", "Diggable", and any tile Type properties ("Grass", "Dirt", "Stone", "Wood")
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

    //a subclass of "SpawnArea" specifically for hardwood generation purposes, including settings for the presence/ratio of stump and log spawns in each area
    public class HardwoodSpawnArea : SpawnArea
    {
        public int StumpFrequency { get; set; }
        public int LogFrequency { get; set; }
        public bool FindExistingHardwoodLocations { get; set; }

        public HardwoodSpawnArea()
            : base()
        {

        }

        public HardwoodSpawnArea(string name, int min, int max, string[] types, string[] include, string[] exclude, int stump, int log, bool find)
            : base(name, min, max, types, include, exclude) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
        {
            StumpFrequency = stump;
            LogFrequency = log;
            FindExistingHardwoodLocations = find;
        }
    }

    //a subclass of "SpawnArea" specifically for ore generation purposes, providing the ability to optionally override each area's skill requirements & spawn chances for ore
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
