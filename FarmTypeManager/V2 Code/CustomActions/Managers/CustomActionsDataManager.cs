using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Manages assets for the custom action system and provides access to the raw data.</summary>
    public static class CustomActionsAssetManager
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The name of the registry asset used to populate <see cref="AssetNames"/>.</summary>
        public static string RegistryAssetName { get; set; } = null;

        /// <summary>A list of asset names to load as <see cref="CustomActionsAsset"/> assets.</summary>
        private static List<string> AssetNames { get; set; } = [];

        /*****************/
        /* Setup methods */
        /*****************/

        /// <summary>Perform all tasks necessary to use this class.</summary>
        /// <param name="helper">This mod's helper instance.</param>
        public static void Initialize(IModHelper helper)
        {
            RegistryAssetName = $"Mods/{helper.ModRegistry.ModID}/Registry";
            
            AssetHelper.SetDefault(RegistryAssetName, () => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)); //handle loading for the registry
            AssetHelper.AddActionOnInvalidate(RegistryAssetName, ReloadAssetNames); //reload asset names whenever the registry asset is invalidated

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        }

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Gets all available custom actions data.</summary>
        /// <returns>All available custom actions data, each paired with a descriptive ID, e.g. its asset name.</returns>
        /// <remarks>This currently only includes data loaded from assets specified in the asset registry. If a future update adds support for data from other sources (e.g. content packs), this method will include those as well.</remarks>
        public static IEnumerable<(string, Dictionary<string,CustomActionsAsset>)> GetAllData()
        {
            foreach (string assetName in AssetNames)
            {
                Dictionary<string, CustomActionsAsset> asset;
                try
                {
                    asset = AssetHelper.GetAsset<Dictionary<string, CustomActionsAsset>>(assetName);
                }
                catch (Exception ex)
                {
                    asset = null;
                    FTMUtility.Monitor.LogOnce($"Failed to load a custom actions asset. Its custom actions will be disabled. Target: \"{assetName}\". Full error message:\n{ex}", LogLevel.Error);
                }
                if (asset != null)
                    yield return (assetName, asset);
            }
            yield break;
        }

        /// <summary>Gets custom actions data from a specific asset in the content system, if available.</summary>
        /// <param name="assetName">The asset's name, e.g. "Characters/Abigail". Case-insensitive (capitalization is ignored).</param>
        /// <returns>The asset and its name, or null if it could not be loaded.</returns>
        public static Dictionary<string,CustomActionsAsset> GetDataFromAsset(string assetName)
        {
            Dictionary<string, CustomActionsAsset> asset;
            try
            {
                asset = AssetHelper.GetAsset<Dictionary<string, CustomActionsAsset>>(assetName);
            }
            catch (Exception ex)
            {
                asset = null;
                FTMUtility.Monitor.LogOnce($"Failed to load a custom actions asset. Its custom actions will be disabled. Target: \"{assetName}\". Full error message:\n{ex}", LogLevel.Error);
            }
            return asset;
        }

        /*******************/
        /* Private methods */
        /*******************/

        /// <summary>Clear <see cref="AssetNames"/>, repopulate it from the registry asset, and register each asset with <see cref="AssetHelper"/>.</summary>
        private static void ReloadAssetNames()
        {
            AssetNames.Clear();
            Dictionary<string, string> registry;
            try
            {
                registry = AssetHelper.GetAsset<Dictionary<string, string>>(RegistryAssetName);
            }
            catch (Exception ex)
            {
                FTMUtility.Monitor.LogOnce($"Failed to load the custom asset registry. All custom actions will be disabled. Target: \"{RegistryAssetName}\". Full error message:\n{ex}", LogLevel.Error);
                return;
            }

            foreach (var entry in registry)
            {
                AssetNames.Add(entry.Key);

                //tell AssetHelper to load this asset
                if (!AssetHelper.HasDefault(entry.Key))
                    AssetHelper.SetDefault(entry.Key, () => new Dictionary<string, CustomActionsAsset>(StringComparer.OrdinalIgnoreCase));
            }
        }

        /// <summary>When the game is fully loaded, finish any remaining asset initialization.</summary>
        /// <remarks>
        /// <para>Assets loaded at this time will generally miss any edits by Content Patcher packs, which happen 1-2 ticks later. However, loading now provides support for very early triggers, if needed by other mods.</para>
        /// <para>When Content Patcher is ready, it will invalidate edited assets and clear their content system caches. <see cref="AssetHelper"/> will clear local caches and reload assets as needed.</para>
        /// </remarks>
        private static void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ReloadAssetNames(); //perform an initial load, enabling automatic updates via invalidation
        }
    }
}
