using StardewModdingAPI;

namespace FarmTypeManager
{
    /// <summary>Represents this mod's config.json file settings.</summary>
    public class ModConfig
    {
        /// <summary>Controls whether this mod's console commands are enabled.</summary>
        public bool EnableConsoleCommands { get; set; } = true;
        /// <summary>Controls whether this mod's content packs are enabled.</summary>
        public bool EnableContentPacks { get; set; } = true;
        /// <summary>Controls whether this mod generates any trace-level messages in the console and log files.</summary>
        public bool EnableTraceLogMessages { get; set; } = true;
        /// <summary>Controls whether Expanded Preconditions Utility (EPU) will generate debug messages about this mod's precondition checks.</summary>
        public bool EnableEPUDebugMessages { get; set; } = false;
        /// <summary>Controls the maximum number of monsters this mod will spawn simultaneously at an in-game location. If this many monsters currently exist, any further spawns will be skipped. If null, unlimited monsters are allowed.</summary>
        public int? MonsterLimitPerLocation { get; set; } = null;

        public ModConfig()
        {
            if (Constants.TargetPlatform == GamePlatform.Android) //if a new config.json is created on an Android device
            {
                MonsterLimitPerLocation = 50; //default to the community-recommended default monster limit (note: this info is outdated)
            }
        }
    }
}