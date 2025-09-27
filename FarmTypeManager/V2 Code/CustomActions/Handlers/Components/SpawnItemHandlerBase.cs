using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A base for handlers that spawn a set of items at in-game locations.</summary>
    public abstract class SpawnItemHandlerBase<TSettings> : LocationHandlerBase<TSettings>, ICustomActionHandler where TSettings : class, ILocationSettings, ITileSettings, ISpawnItemSettings, ITimesToPerformSettings
    {
        /***********************/
        /* LocationHandlerBase */
        /***********************/

        protected override bool TryActionAtLocation(GameLocation location, TSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfItems, out string error)
        {
            queryContext = new(location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random, queryContext.IgnoreQueryKeys, queryContext.CustomFields); //use the current location for context

            var tileQuery = new TileCondition(location, settings.TileCondition);
            var tiles = tileQuery.GetTiles(true).GetEnumerator();

            ItemQueryContext itemContext = new(location, queryContext.Player, queryContext.Random, $"FTM custom action handler. Trigger: \"{triggerContext.Trigger}\". Handler type: \"{GetType()}\".");

            int totalSpawned = 0;
            foreach (Item item in settings.CreateItems(queryContext, itemContext, numberOfItems, false))
            {
                if (!tiles.MoveNext()) //if no more tiles exist, stop
                    break;

                Vector2 tile = tiles.Current;

                if (TryPlaceItem(location, tile, item, out string placementError))
                {
                    totalSpawned++;
                }
                else if (placementError != null) //if placement failed and also returned error text
                {
                    error = $"Failed to spawn an item due to a placement error. Item ID: \"{item.QualifiedItemId}\". Error: \"{placementError}\".";
                    return false;
                }
            }

            if (totalSpawned > 0 || FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.Log($"Spawned {totalSpawned} items at {location.NameOrUniqueName}.", LogLevel.Trace);

            error = null;
            return true;
        }

        /********************/
        /* Abstract methods */
        /********************/

        /// <summary>Places an item on a specified tile, if possible.</summary>
        /// <typeparam name="TItem">The type of <see cref="Item"/> being placed.</typeparam>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="tile">The tile to use.</param>
        /// <param name="item">The item to place.</param>
        /// <param name="placementError">Error text describing why an error occured during placement, e.g. the item type was invalid. Null if placement succeeeded, or if it only failed due to obstructions.</param>
        /// <returns>True if the item was successfully placed. False if placement was obstructed, or if an error occurred.</returns>
        /// <remarks>If error is null, this method should NOT cause the handler to return false. Failure due to obstructions shouldn't be treated as an error. Returning false with non-null error text indicates an error (e.g. an invalid item type).</remarks>
        protected abstract bool TryPlaceItem<TItem>(GameLocation location, Vector2 tile, TItem item, out string placementError) where TItem : Item;
    }
}
