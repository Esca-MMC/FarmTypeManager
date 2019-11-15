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
        private void AddSMAPIEvents(IModHelper helper)
        {
            //tell SMAPI to run event methods when necessary
            helper.Events.GameLoop.DayStarted += DayStarted;
            helper.Events.GameLoop.TimeChanged += TimeChanged;
            helper.Events.GameLoop.DayEnding += DayEnding;
            helper.Events.GameLoop.ReturnedToTitle += ReturnedToTitle;
        }
    }
}
