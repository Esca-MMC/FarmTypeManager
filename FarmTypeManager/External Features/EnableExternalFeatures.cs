using FarmTypeManager.ExternalFeatures.ContentPatcherTokens;
using FarmTypeManager.ExternalFeatures.GameStateQueries;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Enables any external features provided by this mod, e.g. Content Patcher tokens.</summary>
        private void EnableExternalFeatures(IModHelper helper)
        {
            //enable Content Patcher tokens
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched_EnableContentPatcherTokens;

            //enable game state queries (GSQs)
            GSQ_LocationExists.Enable(helper);
            GSQ_LocationIsActive.Enable(helper);
            GSQ_NumberOfMonsters.Enable(helper);
        }

        [EventPriority(EventPriority.Low)] //CP's API is initialized at GameLaunched, so delay this until afterward
        private void GameLoop_GameLaunched_EnableContentPatcherTokens(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
        {
            try
            {
                Utility.ContentPatcherAPI.RegisterToken(Utility.Manifest, "NumberOfMonsters", new Token_NumberOfMonsters());
            }
            catch (Exception ex)
            {
                Monitor.Log($"An error occurred while initializing Content Patcher tokens. Content packs that rely on FTM's tokens might not work correctly. Full error message: \n{ex.ToString()}", LogLevel.Error);
            }
        }
    }
}
