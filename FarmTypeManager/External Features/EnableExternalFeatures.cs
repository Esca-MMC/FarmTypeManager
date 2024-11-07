using StardewModdingAPI;
using FarmTypeManager.ExternalFeatures.ContentPatcherTokens;
using System;
using StardewModdingAPI.Events;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Enables any external features provided by this mod, e.g. Content Patcher tokens.</summary>
        private void EnableExternalFeatures(IModHelper helper)
        {
            //enable Content Patcher tokens
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched_EnableContentPatcherTokens;
        }

        [EventPriority(EventPriority.Low)] //CP's API is initialized at GameLaunched, so delay this until afterward
        private void GameLoop_GameLaunched_EnableContentPatcherTokens(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            try
            {
                Utility.ContentPatcherAPI.RegisterToken(Utility.Manifest, "NumberOfMonsters", new NumberOfMonstersToken());
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while initializing Content Patcher tokens. Content packs that rely on FTM's tokens might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
