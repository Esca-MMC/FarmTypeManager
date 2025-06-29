using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;

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

        /// <summary>A list of asset names to load as <see cref="CustomActionsData"/> assets.</summary>
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



        /*******************/
        /* Internal events */
        /*******************/

        /// <summary>Clear <see cref="AssetNames"/>, repopulate it from the registry asset, and register each asset with <see cref="AssetHelper"/>.</summary>
        private static void ReloadAssetNames()
        {
            var registry = AssetHelper.GetAsset<Dictionary<string, string>>(RegistryAssetName);
            AssetNames.Clear();
            foreach (var entry in registry)
            {
                AssetNames.Add(entry.Key);

                //tell AssetHelper to load this asset
                if (!AssetHelper.HasDefault(entry.Key))
                    AssetHelper.SetDefault(entry.Key, () => new Dictionary<string, CustomActionsData>(StringComparer.OrdinalIgnoreCase));
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
