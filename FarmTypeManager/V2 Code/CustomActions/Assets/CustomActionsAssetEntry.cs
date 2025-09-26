using StardewValley;
using StardewValley.GameData;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of data that describes when to trigger customizable actions, which actions to trigger, and settings specific to each action.</summary>
    public class CustomActionsAssetEntry : ITimesToPerformSettings
    {
        /*********/
        /* Enums */
        /*********/

        /// <summary>The available values of <see cref="ActionMode"/>.</summary>
        public enum ActionModes
        {
            /// <summary>Every valid action will be performed each time.</summary>
            All,
            /// <summary>One random action will be performed each time, with chances based on <see cref="CustomActionData.Weight"/>.</summary>
            Random
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>A set of triggers from the Data/TriggerActions system. When one of these triggers occurs, this entry's actions will be performed.</summary>
        /// <remarks>Case-insensitive. Multiple triggers should be separated by spaces.</remarks>
        public string Trigger { get; set; } = null;

        /// <summary>Whether this action should only be performed while this client is the host player (i.e. while playing in single player mode or hosting a multiplayer session).</summary>
        /// <remarks>
        /// <para>This prevents actions while playing as a farmhand (i.e. joining a multiplayer session). It does not necessarily prevent actions while outside of a game, e.g. on the main menu, and should only be used for in-game actions.</para>
        /// <para>This setting is true by default because FTM's primary use is to spawn things in-game. Most of its actions are more reliable and intuitive when performed only by the host client.</para>
        /// </remarks>
        public bool HostOnly { get; set; } = true;

        /// <summary>The mail flag to use as a completion marker for this entry, if any.</summary>
        /// <remarks>
        /// <para>In general, this setting causes the entry to only be performed once. It is similar in function to <see cref="StardewValley.GameData.TriggerActionData.MarkActionApplied"/>, but uses mail flags.</para>
        /// <para>
        /// If this value is set, then when this entry's actions are performed, the value will be set as a mail flag for the current local player.
        /// If the player already has the mail flag, this entry's custom actions will be skipped.
        /// </para>
        /// </remarks>
        public string MarkAppliedWithFlag { get; set; } = null;

        /// <summary>A <see cref="GameStateQuery"/> condition. If it's null or it returns true when checked, this entry's actions can be performed.</summary>
        public string Condition { get; set; } = null;

        /// <summary>The method to use when selecting actions to perform from <see cref="CustomActions"/>.</summary>
        public ActionModes ActionMode { get; set; } = ActionModes.All;

        /***************************/
        /* ITimesToPerformSettings */
        /***************************/

        /// <summary>The minimum number of times to perform actions from this entry when it's triggered.</summary>
        public int MinTimes { get; set; } = 1;

        /// <summary>The maximum number of times to perform actions from this entry when it's triggered.</summary>
        public int MaxTimes { get; set; } = 1;

        public List<QuantityModifier> TimesModifiers { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public QuantityModifier.QuantityModifierMode TimesModifierMode { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /***********************/
        /* Custom actions list */
        /***********************/
        //NOTE: This is in a separate section at the bottom for organizational reasons. Raw exports of this asset, e.g. from Content Patcher's "patch export" command, would otherwise split settings above/below this list of large objects.

        /// <summary>A list of custom actions to perform when this entry is triggered.</summary>
        public Dictionary<string, CustomActionData> CustomActions { get; set; } = null;
    }
}
