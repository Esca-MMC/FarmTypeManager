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
        /// <summary>True if QuickSave's save process is currently being handled by this mod. Used to avoid redundant save data handling.</summary>
        public static bool QuickSaveIsSaving { get; set; } = false;

        /// <summary>Raised after the game is launched, right before the first update tick. This happens once per game session (unrelated to loading saves).</summary>
        public void EnableQuickSave(object sender, GameLaunchedEventArgs e)
        {
            //Save Anywhere: pass compatibility events for handling this mod's custom classes
            var quickSave = Utility.Helper.ModRegistry.GetApi<IQuickSaveAPI>("DLX.QuickSave");
            if (quickSave != null) //if the API was accessed successfully
            {
                Utility.Monitor.Log("QuickSave API loaded. Sending compatibility events.", LogLevel.Trace);
                quickSave.SavingEvent += QuickSave_SavingEvent;
                quickSave.SavedEvent += QuickSave_SavedEvent;
            }
        }

        private void QuickSave_SavingEvent(object sender, ISavingEventArgs e)
        {
            Utility.GameIsSaving = true;

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, do nothing

            QuickSaveIsSaving = true;
            SkipDayStartedEvents = true;

            BeforeMidDaySave();
        }

        private void QuickSave_SavedEvent(object sender, ISavedEventArgs e)
        {
            Utility.GameIsSaving = false;

            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, do nothing

            AfterMidDaySave();

            QuickSaveIsSaving = false;
        }
    }
}
