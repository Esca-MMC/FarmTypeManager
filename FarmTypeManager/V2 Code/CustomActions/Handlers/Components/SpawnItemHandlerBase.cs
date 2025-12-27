using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
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
            ItemQueryContext itemContext = new(location, queryContext.Player, queryContext.Random, $"FTM custom action handler. Trigger: \"{triggerContext.Trigger}\". Handler type: \"{GetType()}\".");
            
            Dictionary<Vector2, IEnumerator<Vector2>> SizedTiles = []; //key = the tile size needed to place an item; value = the tile enumerator to use for that size
            int totalSpawned = 0;

            foreach (Item item in settings.CreateItems(queryContext, itemContext, numberOfItems))
            {
                Vector2 size = GetItemSize(item);
                if (!SizedTiles.TryGetValue(size, out var tiles)) //try to get the enumerator for tiles of this size
                {
                    //if the condition for this item size doesn't exist yet, create and store it

                    string modifiedCondition = ModifyTileCondition(settings.TileCondition);
                    if (settings.TileCondition != null && (size.X > 1 || size.Y > 1)) //if a tile condition was provided & this item is larger than 1x1
                        modifiedCondition = ArgUtility.UnsplitQuoteAware(["SIZE", size.X.ToString(), size.Y.ToString(), modifiedCondition], ' '); //add "SIZE X Y" query to the condition with quote adjustments

                    TileCondition tileCondition = new(location, modifiedCondition);
                    tiles = tileCondition.GetTiles(true).GetEnumerator();
                    SizedTiles[size] = tiles;
                }

                if (!tiles.MoveNext()) //if no more tiles exist of this size
                {
                    if (size == Vector2.One) //if this is 1x1, no sizes can be placed anymore, so stop early
                        break;
                    else
                        continue; //skip this item
                }

                Vector2 tile = tiles.Current;

                if (TryPlaceItem(location, tile, item, out string placementError))
                {
                    totalSpawned++;
                    if (FTMUtility.Monitor.IsVerbose)
                        FTMUtility.Monitor.VerboseLog($"Spawned an item. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. Item ID: \"{item.QualifiedItemId}\".");
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

        /***********/
        /* Methods */
        /***********/

        /// <summary>Gets the width and height, in tiles, occupied by the item when placed.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>The width (X) and height (Y) in tile that the item would occupy when placed.</returns>
        protected virtual Vector2 GetItemSize(Item item) => Vector2.One;

        /// <summary>Modifies a tile condition string to include this handler's requirements or optimizations, if any.</summary>
        /// <param name="tileCondition">The tile condition string to modify.</param>
        /// <returns>A modified version of the tile condition.</returns>
        /// <remarks>The base method returns the tile condition without any changes. Some handlers may implement changes to improve performance, avoid problematic tiles, etc.</remarks>
        protected virtual string ModifyTileCondition(string tileCondition) => tileCondition;

        /// <summary>Places an item on a specified tile, if possible.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="tile">The tile to use.</param>
        /// <param name="item">The item to place.</param>
        /// <param name="placementError">Error text describing why an error occured during placement, e.g. the item type was invalid. Null if placement succeeeded, or if it only failed due to obstructions.</param>
        /// <returns>True if the item was successfully placed. False if placement was obstructed, or if an error occurred.</returns>
        /// <remarks>If <paramref name="placementError"/> is null, a false return value should NOT be treated as an error by the caller. "Safe" failures can occur due to obstructed tiles, etc. Returning false with non-null error text indicates an error (e.g. due to invalid item types, caught exceptions, etc).</remarks>
        protected abstract bool TryPlaceItem(GameLocation location, Vector2 tile, Item item, out string placementError);
    }
}
