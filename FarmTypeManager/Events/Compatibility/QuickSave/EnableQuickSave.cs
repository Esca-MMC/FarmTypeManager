using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FarmTypeManager
{
    public interface IQuickSaveAPI
    {
        /* Save Event Order:
         * 1. QS-Saving (IsSaving = true) 
         * 2. QS-Saved (IsSaving = false)
         */

        /// <summary>Fires before a Quicksave is being created</summary>
        public event SavingDelegate SavingEvent;
        /// <summary>Fires after a Quicksave has been created</summary>
        public event SavedDelegate SavedEvent;
        public bool IsSaving { get; }

        /* Load Event Order:
         * 1. QS-Loading (IsLoading = true)
         * 2. SMAPI-LoadStageChanged
         * 3. SMAPI-SaveLoaded & SMAPI-DayStarted
         * 4. QS-Loaded (IsLoading = false)
         */

        /// <summary>Fires before a Quicksave is being loaded</summary>
        public event LoadingDelegate LoadingEvent;
        /// <summary>Fires after a Quicksave was loaded</summary>
        public event LoadedDelegate LoadedEvent;
        public bool IsLoading { get; }

        public delegate void SavingDelegate(object sender, ISavingEventArgs e);
        public delegate void SavedDelegate(object sender, ISavedEventArgs e);
        public delegate void LoadingDelegate(object sender, ILoadingEventArgs e);
        public delegate void LoadedDelegate(object sender, ILoadedEventArgs e);
    }
    public interface ISavingEventArgs { }
    public interface ISavedEventArgs { }
    public interface ILoadingEventArgs { }
    public interface ILoadedEventArgs { }

    public partial class ModEntry : Mod
    {
        public static bool QuickSaveIsSaving => QuickSaveAPI?.IsSaving ?? false;
        public static bool QuickSaveIsLoading => QuickSaveAPI?.IsLoading ?? false;
        /// <summary> A suffix that will be appended to FTMs savefiles whenever QS is saving or loading to store data separately for QuickSaves. </summary>
        public static string QSSaveFileSuffix => QuickSaveIsLoading || QuickSaveIsSaving ? "_QuickSave" : "";

        private static IQuickSaveAPI QuickSaveAPI = null;

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).</summary>
        public void EnableQuickSave(object sender, GameLaunchedEventArgs e)
        {
            QuickSaveAPI = Utility.Helper.ModRegistry.GetApi<IQuickSaveAPI>("DLX.QuickSave");
            if (QuickSaveAPI is null) { return; }

            Utility.Monitor.Log("QuickSave API loaded. Sending compatibility events.", LogLevel.Trace);
            QuickSaveAPI.SavingEvent += QuickSave_SavingEvent;
            QuickSaveAPI.SavedEvent += QuickSave_SavedEvent;
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
