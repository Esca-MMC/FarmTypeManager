using FarmTypeManager.CustomActions;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Triggers;
using System;

namespace FarmTypeManager
{
    /// <summary>A Harmony patch that passes triggers and context information to this mod's custom action system.</summary>
    public static class HarmonyPatch_TriggerCustomActions
    {
        /// <summary>Applies this Harmony patch to the game through the provided instance.</summary>
        /// <param name="harmony">This mod's Harmony instance.</param>
        public static void ApplyPatch(Harmony harmony)
        {
            FTMUtility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_TriggerCustomActions)}\": postfixing SDV method \"TriggerActionManager.Raise\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(TriggerActionManager), nameof(TriggerActionManager.Raise), [typeof(string), typeof(object[]), typeof(GameLocation), typeof(Farmer), typeof(Item), typeof(Item)]),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_TriggerCustomActions), nameof(Raise_Postfix))
            );
        }
            
        /// <summary>Passes any raised triggers and contextual data to <see cref="CustomActions.CustomActionManager"/> for use with custom actions.</summary>
        /// <inheritdoc cref="TriggerActionManager.Raise"/>
        public static void Raise_Postfix(string trigger, object[] triggerArgs, GameLocation location, Farmer player, Item targetItem, Item inputItem)
        {
            try
            {
                GameStateQueryContext queryContext = new(location, player, targetItem, inputItem, null);
                TriggerActionContext triggerContext = new(trigger, triggerArgs ?? [], null); //note: triggerArgs shouldn't be left null, according to its docs, but its constructor doesn't enforce that

                CustomActionManager.PerformActionsByTrigger(queryContext, triggerContext);
            }
            catch (Exception ex)
            {
                FTMUtility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_TriggerCustomActions)}\" has encountered an error. This mod's custom action system might stop working. Full error message: \n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
