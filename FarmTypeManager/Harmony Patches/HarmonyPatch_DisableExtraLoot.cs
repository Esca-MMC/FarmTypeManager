using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static HarmonyLib.Code;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A Harmony patch that disables certain loot (a.k.a. item drops) when monsters are defeated, depending on a specific modData flag.</summary>
        public class HarmonyPatch_DisableExtraLoot
        {
            /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
            /// <param name="harmony">This mod's Harmony instance.</param>
            public static void ApplyPatch(Harmony harmony)
            {
                try
                {
                    //apply Harmony patches
                    Utility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_DisableExtraLoot)}\": transpiling SDV method \"GameLocation.monsterDrop\".", LogLevel.Trace);
                    harmony.Patch(
                        original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop)),
                        transpiler: new HarmonyMethod(typeof(HarmonyPatch_DisableExtraLoot), nameof(monsterDrop_Transpiler))
                    );
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DisableExtraLoot)}\" failed to apply. Modded monsters might drop non-modded loot in some cases. Full error message: \n{ex.ToString()}", LogLevel.Error);
                }
            }

            /// <summary>Disables extra loot from the Burglar's Ring for monsters with a specific modData flag.</summary>
            /// <param name="instructions">The original instructions for the target method.</param>
            /// <remarks>
            /// Old C#:
            ///     if (who.isWearingRing("526"))
            ///     
            /// New C#:
            ///     if (ModifiedBurglarsRingCheck(who.isWearingRing("526"), monster)
            ///     
            /// Old IL:
            /// 	IL_0019: ldarg.s who
            ///     IL_001b: ldstr "526"
	        ///     IL_0020: callvirt instance bool StardewValley.Farmer::isWearingRing(string)
            ///     IL_0025: brfalse.s IL_0033
            ///     
            /// New IL:
            /// 	IL_0019: ldarg.s who
            ///     IL_001b: ldstr "526"
	        ///     IL_0020: callvirt instance bool StardewValley.Farmer::isWearingRing(string)
            ///         (?): ldarg.1
            ///         (?): call bool FarmTypeManager.HarmonyPatch_DisableExtraLoot::ModifiedBurglarsRingCheck(bool, StardewValley.Monsters.Monster)
            ///     IL_0025: brfalse.s IL_0033
            /// </remarks>
            public static IEnumerable<CodeInstruction> monsterDrop_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                try
                {
                    List<CodeInstruction> patched = new List<CodeInstruction>(instructions);

                    var ringCheckMethodInfo = AccessTools.Method(typeof(HarmonyPatch_DisableExtraLoot), nameof(ModifiedBurglarsRingCheck));

                    for (int x = patched.Count - 2; x >= 0; x--) //loop backward, starting 1 instruction before the last
                    {
                        if
                        (
                            patched[x].opcode == OpCodes.Ldstr && patched[x].operand?.ToString().Equals("526", StringComparison.Ordinal) == true //if this loads the string "526" (unqualified item ID of Burglar's Ring)
                            && (patched[x+1].opcode == OpCodes.Call || patched[x + 1].opcode == OpCodes.Callvirt) && patched[x + 1].operand?.ToString().Contains("isWearingRing", StringComparison.OrdinalIgnoreCase) == true //and if this calls the "isWearingRing" method
                        )
                        {
                            patched.InsertRange(x+2,
                                [ 
                                    new CodeInstruction(OpCodes.Ldarg_1), //add the "monster" arg to the stack
                                    new CodeInstruction(OpCodes.Call, ringCheckMethodInfo) //call this method to conditionally replace the result of "isWearingRing"
                                ]);
                        }
                    }

                    return patched;
                }
                catch (Exception ex)
                {
                    Utility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DisableExtraLoot)}\" has encountered an error. Modded monsters might lose their immunity to certain attacks. Full error message: \n{ex.ToString()}", LogLevel.Error);
                    return instructions;
                }
            }

            /// <summary>Modifies the result of a check for the Burglar's Ring, based on a related modData flag.</summary>
            /// <param name="isWearingBurglarsRing">The original result of a check for the Burglar's Ring.</param>
            /// <param name="monster">The monster whose loot (a.k.a. item drops) are being checked.</param>
            /// <returns>False if the monster's extra loot from the Burglar's Ring should be skipped. Otherwise returns the provided bool without changes.</returns>
            private static bool ModifiedBurglarsRingCheck(bool isWearingBurglarsRing, Monster monster)
            {
                if (isWearingBurglarsRing)
                {
                    if (monster?.modData.TryGetValue(Utility.ModDataKeys.ExtraLoot, out string extraLoot) == true)
                        if (extraLoot?.StartsWith("f", StringComparison.OrdinalIgnoreCase) == true) //if the extra loot value is negative ("F", "false", etc)
                            return false; //override the original result, as if the player is NOT wearing a Burglar's Ring
                }

                return isWearingBurglarsRing; //use the original result
            }
        }
    }
}
