using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Add this mod's event methods to SMAPI's event handlers. This should be performed once during mod entry.</summary>
        private void AddSMAPIEvents(IModHelper helper)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything; most of this mod's functions should be limited to the host player

            //tell SMAPI to run event methods when necessary
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.TimeChanged += TimeChanged;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
            helper.Events.Multiplayer.ModMessageReceived += ModMessageReceived;
        }
    }
}
