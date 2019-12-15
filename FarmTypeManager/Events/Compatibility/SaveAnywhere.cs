using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    /// <summary>An interface describing the necessary content of Save Anywhere's API.</summary>
    public interface ISaveAnywhereAPI
    {
        void addBeforeSaveEvent(string ID, Action BeforeSave);
        void addAfterSaveEvent(string ID, Action BeforeSave);
    }

    public partial class ModEntry : Mod
    {
        /// <summary>Handles the saving and removal of custom classes that can't be properly handled by SDV.</summary>
        public static void SaveAnywhere_BeforeSave()
        {
            Utility.Monitor.Log($"Save Anywhere pre-save event started. Processing save data and removing custom classes...", LogLevel.Trace);

            if (Context.IsMainPlayer != true)
            {
                Utility.Monitor.Log($"Player is a multiplayer farmhand, so no spawn data exists. Save Anywhere pre-save event complete.", LogLevel.Trace);
                return;
            }

            if (Utility.FarmDataList == null)
            {
                Utility.Monitor.Log($"No farm data currently exists. Save Anywhere pre-save event complete.", LogLevel.Trace);
                return;
            } 

            foreach (FarmData data in Utility.FarmDataList) //for each set of farm data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Utility.Monitor.VerboseLog($"Processing save data for content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Utility.Monitor.VerboseLog($"Processing save data for FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ProcessObjectExpiration(data.Save, false); //remove expired objects, but DO NOT update expiration data

                if (data.Pack != null) //if this data is from a content pack
                {
                    data.Pack.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), data.Save); //update the save file for that content pack
                }
                else //this data is from this mod's own folders
                {
                    Utility.Helper.Data.WriteJsonFile(Path.Combine("data", $"{Constants.SaveFolderName}_SaveData.save"), data.Save); //update the save file in this mod's own folders
                }
            }

            Utility.Monitor.Log($"Save Anywhere pre-save event complete.", LogLevel.Trace);
        }

        /// <summary>Handles the respawning of custom classes that were removed by the BeforeSave event.</summary>
        public static void SaveAnywhere_AfterSave()
        {
            Utility.Monitor.Log($"Save Anywhere post-save event started. Respawning custom classes...", LogLevel.Trace);

            foreach (FarmData data in Utility.FarmDataList) //for each loaded set of data
            {
                if (data.Pack != null) //if this data is from a content pack
                {
                    Utility.Monitor.VerboseLog($"Checking objects from content pack: {data.Pack.Manifest.Name}");
                }
                else //this data is from this mod's own folders
                {
                    Utility.Monitor.VerboseLog($"Checking objects from FarmTypeManager/data/{Constants.SaveFolderName}_SaveData.save");
                }

                Utility.ReplaceProtectedSpawns(data.Save); //protect unexpired spawns listed in the save data
            }

            Utility.Monitor.Log($"Save Anywhere post-save event complete.", LogLevel.Trace);
        }
    }
}
