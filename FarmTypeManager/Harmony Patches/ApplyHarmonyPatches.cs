using HarmonyLib;

using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>
        /// Applies any Harmony patches used by this mod.
        /// </summary>
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony(this.ModManifest.UniqueID); //create this mod's Harmony instance

            //apply all patches
            HarmonyPatch_DisableFurniturePickup.ApplyPatch(harmony);
            HarmonyPatch_OptimizeMonsterCode.ApplyPatch(harmony);
            HarmonyPatch_ToggleExtraLoot.ApplyPatch(harmony);
            HarmonyPatch_UpdateCursorOverPlacedItem.ApplyPatch(harmony);
        }
    }
}
