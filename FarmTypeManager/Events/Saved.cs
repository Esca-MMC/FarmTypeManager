﻿using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Tasks performed after the game saves.</summary>
        private void GameLoop_Saved(object sender, SavedEventArgs e)
        {
            Utility.GameIsSaving = false;

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't do anything
            if (Utility.DayIsEnding || SaveAnywhereIsSaving || QuickSaveIsSaving) { return; } //if a specialized save process is already handling this, don't do anything

            AfterMidDaySave();
        }

        /// <summary>Loads and respawns custom entities after a mid-day save event.</summary>
        private void AfterMidDaySave()
        {
            Utility.Monitor.Log($"Mid-day save event ended. Restoring custom objects/data.", LogLevel.Trace);

            //note: do not clear Utility.TimedSpawns here; that should only happen when in the DayStarted event, which currently happens whenever saves are loaded (even "mid-day" ones)

            Utility.MonsterTracker.Clear(); //clear stored monster information before they are respawned (note: all of this mod's monsters should be removed before saving)

            foreach (FarmData data in Utility.FarmDataList) //for each loaded set of data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Utility.Monitor.VerboseLog($"Checking objects from content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Utility.Monitor.VerboseLog($"Checking objects from FarmTypeManager/data/{Constants.SaveFolderName}{QSSaveFileSuffix}_SaveData.save");
                }

                Utility.ReplaceProtectedSpawns(data.Save); //protect unexpired spawns listed in the save data
            }
        }
    }
}
