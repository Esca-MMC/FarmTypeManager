using FarmTypeManager.HarmonyPatches;
using FarmTypeManager.Utilities;
using HarmonyLib;
using StardewModdingAPI;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Tasks performed when the mod initially loads.</summary>
        public override void Entry(IModHelper helper)
        {
            //pass SMAPI utilities to the Utility class for use throughout ModEntry (deprecated in 2.0+)
            Utility.Monitor.IMonitor = Monitor;
            Utility.Helper = helper;
            Utility.Manifest = ModManifest;

            //pass SMAPI utilities to the static class for global use
            Properties.Monitor.IMonitor = Monitor;
            Properties.Helper = helper;
            Properties.Manifest = ModManifest;

            Utility.LoadModConfig(); //attempt to load the config.json ModConfig file

            if (Utility.MConfig?.EnableConsoleCommands == true) //if enabled, pass the mod's console command methods to the helper
            {
                helper.ConsoleCommands.Add("whereami", "Outputs coordinates and other information about the player's current location.", WhereAmI);
                helper.ConsoleCommands.Add("list_monsters", "Outputs a list of available monster types, including custom types loaded by other mods.", ListMonsters);
                helper.ConsoleCommands.Add("remove_items", "Removes an item or object in front of the player.\nUse \"remove_items X Y\" to remove an item from a specific tile.\nUse \"remove_items permanent\" to remove any FTM items from your location that cannot be removed normally (due to the \"CanBePickedUp\" setting).", RemoveItems);
            }

            AddSMAPIEvents(helper);
            EnableExternalFeatures(helper);

            //apply all Harmony patches
            var harmony = new Harmony(ModManifest.UniqueID); //create this mod's Harmony instance
            HarmonyPatch_DisableFurniturePickup.ApplyPatch(harmony);
            HarmonyPatch_InstantKillImmunity.ApplyPatch(harmony);
            HarmonyPatch_OptimizeMonsterCode.ApplyPatch(harmony);
            HarmonyPatch_StunImmunity.ApplyPatch(harmony);
            HarmonyPatch_ToggleExtraLoot.ApplyPatch(harmony);
            HarmonyPatch_DropMonsterLoot.ApplyPatch(harmony);
            HarmonyPatch_TriggerCustomActions.ApplyPatch(harmony);
            HarmonyPatch_UpdateCursorOverPlacedItem.ApplyPatch(harmony);
        }
    }
}