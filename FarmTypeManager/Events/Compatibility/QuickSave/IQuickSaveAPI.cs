//External mod API file
//Mod name: QuickSave
//ID: DLX.QuickSave
//Source URL of latest update: https://gitlab.com/delixx/stardew-valley/quicksave/-/blob/main/QuickSave/API/IQuickSaveAPI.cs

namespace QuickSave.API
{
    public interface IQuickSaveAPI
    {
        /* Save Event Order:
         * 1. QS-Saving (IsSaving = true) 
         * 2. QS-SavingExtraDataEvent (IsSaving = true)
         * 3. QS-Saved (IsSaving = false)
         */

/// <summary>Fires before a Quicksave is being created</summary>
public event SavingDelegate SavingEvent;
        /// <summary>Fires while a QuickSave is being created, directly after the ExtraSaveData was generated and before it is written to the savefile.</summary>
        public event SavingExtraDataDelegate SavingExtraDataEvent;
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
        public delegate void SavingExtraDataDelegate(object sender, ISavingExtraDataEventArgs e);
        public delegate void SavedDelegate(object sender, ISavedEventArgs e);
        public delegate void LoadingDelegate(object sender, ILoadingEventArgs e);
        public delegate void LoadedDelegate(object sender, ILoadedEventArgs e);
    }
    public interface ISavingEventArgs { }
    /// <summary>Contains the ExtraSaveData: <seealso cref="SavingExtraDataEventArgs"/></summary>
    public interface ISavingExtraDataEventArgs { }
    /// <summary>Contains the ExtraSaveData: <seealso cref="SavedEventArgs"/></summary>
    public interface ISavedEventArgs
    {
    }
    public interface ILoadingEventArgs { }
    /// <summary>Contains the ExtraSaveData: <seealso cref="LoadedEventArgs"/></summary>
    public interface ILoadedEventArgs { }
}
