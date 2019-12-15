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
        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).</summary>
        public void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            //Save Anywhere: pass compatibility events for handling this mod's custom classes
            ISaveAnywhereAPI saveAnywhere = Utility.Helper.ModRegistry.GetApi<ISaveAnywhereAPI>("Omegasis.SaveAnywhere");
            if (saveAnywhere != null) //if the API was accessed successfully
            {
                Utility.Monitor.Log("Save Anywhere API loaded. Sending compatibility events.", LogLevel.Trace);
                saveAnywhere.addBeforeSaveEvent(ModManifest.UniqueID, SaveAnywhere_BeforeSave);
                saveAnywhere.addAfterSaveEvent(ModManifest.UniqueID, SaveAnywhere_AfterSave);
            }
        }
    }
}
