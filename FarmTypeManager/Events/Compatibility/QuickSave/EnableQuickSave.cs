using FarmTypeManager.Utilities;
using QuickSave.API;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        public static bool QuickSaveIsSaving => FTMUtility.ModAPIs.QuickSaveAPI?.IsSaving ?? false;
        public static bool QuickSaveIsLoading => FTMUtility.ModAPIs.QuickSaveAPI?.IsLoading ?? false;
        /// <summary> A suffix that will be appended to FTMs savefiles whenever QS is saving or loading to store data separately for QuickSaves. </summary>
        public static string QSSaveFileSuffix => QuickSaveIsLoading || QuickSaveIsSaving ? "_QuickSave" : "";

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).</summary>
        public void EnableQuickSave(object sender, GameLaunchedEventArgs e)
        {
            if (FTMUtility.ModAPIs.QuickSaveAPI == null)
                return;

            FTMUtility.Monitor.Log("QuickSave API found. Adding compatibility events for FTM content packs.", LogLevel.Trace);
            FTMUtility.ModAPIs.QuickSaveAPI.SavingEvent += QuickSave_SavingEvent;
            FTMUtility.ModAPIs.QuickSaveAPI.SavedEvent += QuickSave_SavedEvent;
        }

        private void QuickSave_SavingEvent(object sender, ISavingEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            Utility.GameIsSaving = true;

            BeforeMidDaySave();
        }

        private void QuickSave_SavedEvent(object sender, ISavedEventArgs e)
        {
            if (!Context.IsMainPlayer) { return; }

            Utility.GameIsSaving = false;

            AfterMidDaySave();
        }
    }
}
