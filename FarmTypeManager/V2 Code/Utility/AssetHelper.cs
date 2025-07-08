using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    /// <summary>A helper used to load assets from the content system. Provides caching, type handling, thread safety, etc.</summary>
    public static class AssetHelper
    {
        /**********************/
        /* Private properties */
        /**********************/

        /// <summary>A set of asset names and their most recently updated instances.</summary>
        private static Dictionary<string, object> Cache { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>A set of asset names and constructors for their default instances.</summary>
        private static Dictionary<string, Func<object>> Defaults { get; } = new Dictionary<string, Func<object>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>A set of asset names and actions to perform when that asset's local cache is invalidated.</summary>
        private static Dictionary<string, List<Action>> OnInvalidate { get; } = new Dictionary<string, List<Action>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>This mod's SMAPI helper instance.</summary>
        private static IModHelper Helper { get; set; } = null;

        /// <summary>True if this class is initialized and ready to use.</summary>
        private static bool Initialized { get; set; } = false;

        /// <summary>A lock used to prevent multiple threads simultaneously loading data.</summary>
        private static object LoadLock { get; set; } = new();

        /*****************/
        /* Setup methods */
        /*****************/

        /// <summary>Performs required setup tasks for this class.</summary>
        /// <param name="helper">This mod's SMAPI helper instance.</param>
        public static void Initialize(IModHelper helper)
        {
            if (Initialized)
                return;

            //store args
            Helper = helper;

            //enable SMAPI events
            helper.Events.Content.AssetRequested += Content_AssetRequested;
            helper.Events.Content.AssetsInvalidated += Content_AssetsInvalidated;

            Initialized = true;
        }

        /// <summary>Checks whether this asset name has a default instance constructor.</summary>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <returns>True if a default instance constructor exists for this asset. False otherwise.</returns>
        public static bool HasDefault(string assetName)
        {
            return Defaults.ContainsKey(assetName);
        }

        /// <summary>Sets a default instance construction method for the named asset, which allows it to be loaded by this class.</summary>
        /// <param name="assetName">The asset name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <param name="getNewDefaultAsset">A method that returns a new default instance for this asset, e.g. a blank dictionary with the appropriate key/value types.</param>
        public static void SetDefault(string assetName, Func<object> getNewDefaultAsset)
        {
            Defaults[assetName] = getNewDefaultAsset; //normalize the asset name and store the default instance
        }

        /// <summary>Gets a default instance of the named asset if one is available.</summary>
        /// <typeparam name="T">The asset's type.</typeparam>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <param name="defaultAsset">A default instance of the asset.</param>
        /// <returns>True if a default instance constructor exists for this asset. False otherwise.</returns>
        public static bool TryGetDefault<T>(string assetName, out T defaultAsset)
        {
            if (Defaults.TryGetValue(assetName, out Func<object> getNewDefaultAsset)) //if this asset has a default to load
            {
                defaultAsset = (T)getNewDefaultAsset.Invoke(); //generate a new default instance of this asset, cast it as the given type, and return it
                return true; //success
            }
            else //if this asset does NOT have a default to load
            {
                defaultAsset = default(T); //return the given type's default value (e.g. null)
                return false; //failure
            }
        }

        /// <summary>Adds an action to perform when the specified asset is invalidated, if this class had an up-to-date cache of the asset.</summary>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <param name="action">The action to perform. This method may be called multiple times to add multiple actions.</param>
        public static void AddActionOnInvalidate(string assetName, Action action)
        {
            if (OnInvalidate.TryGetValue(assetName, out List<Action> actionList) && actionList != null) //if a list already exists for this asset
                actionList.Add(action);
            else
                OnInvalidate[assetName] = [action]; //create a new list with this action added
        }

        /*******************/
        /* Utility methods */
        /*******************/

        /// <summary>Get the most recent version of a game asset.</summary>
        /// <typeparam name="T">The asset's type.</typeparam>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <returns>The latest available version of the asset.</returns>
        public static T GetAsset<T>(string assetName)
        {
            if (Cache.TryGetValue(assetName, out object asset) && asset != null)
                return (T)asset; //cast the cached asset as the specified type, then return it
            else
            {
                T loadedAsset;

                lock (LoadLock) //prevent multiple simultaneous calls to Load (causes errors and/or data corruption)
                {
                    loadedAsset = Helper.GameContent.Load<T>(assetName);
                    Cache[assetName] = loadedAsset;
                }

                return loadedAsset;
            }
        }

        /// <summary>Checks whether this asset's latest version is currently cached.</summary>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <returns>True if the asset's latest version is currently cached. False if it has never been loaded through this class, or if it has been invalidated since it was last retrieved.</returns>
        public static bool HasCachedValue(string assetName)
        {
            return Cache.TryGetValue(assetName, out object asset) && asset != null;
        }

        /// <summary>Clear an asset's cached state, which causes the latest version to be loaded next time it's requested.</summary>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        public static void Invalidate(string assetName)
        {
            if (Cache.TryGetValue(assetName, out object asset) && asset != null)
            {
                Cache[assetName] = null;

                //perform any invalidation actions for this asset
                if (OnInvalidate.TryGetValue(assetName, out var actionList))
                    foreach (var action in actionList)
                        action.Invoke();
            }
        }

        /****************/
        /* SMAPI events */
        /****************/

        /// <summary>When an asset is loaded through the content system, provide a default instance, if applicable.</summary>
        private static void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (TryGetDefault(e.Name.BaseName, out object defaultAsset)) //if a default instance exists for this asset
            {
                e.LoadFrom(() => defaultAsset, StardewModdingAPI.Events.AssetLoadPriority.Low);
            }
        }

        /// <summary>When assets are invalidated by the content system, invalidate them here too, if applicable.</summary>
        private static void Content_AssetsInvalidated(object sender, StardewModdingAPI.Events.AssetsInvalidatedEventArgs e)
        {
            foreach (IAssetName name in e.Names)
                Invalidate(name.BaseName);
        }
    }
}