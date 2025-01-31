using StardewModdingAPI;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Tasks performed when the mod initially loads.</summary>
        public override void Entry(IModHelper helper)
        {
            //pass SMAPI utilities to the Utility class for use throughout ModEntry (deprecated)
            Utility.Monitor.IMonitor = Monitor;
            Utility.Helper = helper;
            Utility.Manifest = ModManifest;

            //pass SMAPI utilities to the FTMUtility class for global use
            FTMUtility.Monitor.IMonitor = Monitor;
            FTMUtility.Helper = helper;
            FTMUtility.Manifest = ModManifest;

            Utility.LoadModConfig(); //attempt to load the config.json ModConfig file

            if (Utility.MConfig?.EnableConsoleCommands == true) //if enabled, pass the mod's console command methods to the helper
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
                helper.ConsoleCommands.Add("list_monsters", "Outputs a list of available monster types, including custom types loaded by other mods.", ListMonsters);
                helper.ConsoleCommands.Add("remove_items", "Removes an item or object in front of the player.\nUse \"remove_items X Y\" to remove an item from a specific tile.\nUse \"remove_items permanent\" to remove any FTM items from your location that cannot be removed normally (due to the \"CanBePickedUp\" setting).", RemoveItems);
            }

            AddSMAPIEvents(helper);
            ApplyHarmonyPatches();
            EnableExternalFeatures(helper);
        }
    }
}