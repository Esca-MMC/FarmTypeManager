using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A base for handlers that spawn one or more in-game instances (e.g. forage objects) on tiles.</summary>
    public abstract class SpawnHandlerBase<TSettings, TSpawn> : BasicHandlerBase<TSettings>, ICustomActionHandler
        where TSettings : class, ILocationSettings, ITimesToPerformSettings, ITileSettings
    {
        /********************/
        /* BasicHandlerBase */
        /********************/

        protected override bool TryActionAtLocation(GameLocation location, TSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfTimes, out string error)
        {
            queryContext = new(location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random, queryContext.IgnoreQueryKeys, queryContext.CustomFields); //use the current location for context

            Dictionary<Vector2, IEnumerator<Vector2>> SizedTiles = []; //key = the tile size needed to place an item; value = the tile enumerator to use for that size
            int totalSpawned = 0;

            foreach (TSpawn instance in CreateInstances(settings, queryContext, triggerContext, numberOfTimes))
            {
                Vector2 size = GetSize(instance);
                if (!SizedTiles.TryGetValue(size, out var tiles)) //try to get the enumerator for tiles of this size
                {
                    //if the condition for this size doesn't exist yet, create and store it

                    string modifiedCondition = ModifyTileCondition(settings.TileCondition);
                    if (settings.TileCondition != null && (size.X > 1 || size.Y > 1)) //if a tile condition was provided & this instance is larger than 1x1
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

                if (TrySpawn(location, tile, instance, out string placementError))
                {
                    totalSpawned++;
                    if (FTMUtility.Monitor.IsVerbose)
                        FTMUtility.Monitor.VerboseLog($"Spawned {LogTextForAnInstance}. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. ID: \"{GetId(instance)}\".");
                }
                else if (placementError != null) //if placement failed and also returned error text
                {
                    error = $"Failed to spawn {LogTextForAnInstance} due to an error during placement. ID: \"{GetId(instance)}\". Error: \"{placementError}\".";
                    return false;
                }
            }

            if (totalSpawned > 0 || FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.Log($"Spawned {totalSpawned} {LogTextForInstances} at {location.NameOrUniqueName}.", LogLevel.Trace);

            error = null;
            return true;
        }

        /******************/
        /* New properties */
        /******************/

        /// <summary>Lower-case singular text for an instance spawned by this handler, e.g. "an item" or "a monster". Used in log messages.</summary>
        protected abstract string LogTextForAnInstance { get; }

        /// <summary>Lower-case plural text for instances spawned by this handler, e.g. "items" or "monsters". Used in log messages.</summary>
        protected abstract string LogTextForInstances { get; }

        /***************/
        /* New methods */
        /***************/

        /// <summary>Creates in-game instances to spawn if possible.</summary>
        /// <param name="settings">The custom action's settings.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <param name="triggerContext">Contextual information about a raised trigger.</param>
        /// <param name="numberOfTimes">The number of instances to generate.</param>
        /// <returns>The instances to spawn at this location, if possible.</returns>
        /// <remarks>There may not be enough valid tiles to spawn every instance. If possible, this method should use yield returns to allow it to stop early.</remarks>
        protected abstract IEnumerable<TSpawn> CreateInstances(TSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfTimes);

        /// <summary>Get a unique identifier for this spawn. Only used in log messages.</summary>
        /// <param name="instance">The spawn to identify. May be null.</param>
        /// <returns>A unique identifier for this spawn.</returns>
        protected virtual string GetId(TSpawn instance) => instance != null ? "[null ID]" : "[null spawn instance]";

        /// <summary>Gets the width and height, in tiles, that a spawned instance would occupy when placed.</summary>
        /// <param name="item">The item to check.</param>
        /// <returns>The width (X) and height (Y) in tiles that a spawned instance would occupy when placed.</returns>
        protected virtual Vector2 GetSize(TSpawn instance) => Vector2.One;

        /// <summary>Modifies a tile condition string to include this handler's requirements or optimizations, if any.</summary>
        /// <param name="tileCondition">The tile condition string to modify.</param>
        /// <returns>A modified version of the tile condition.</returns>
        /// <remarks>The base method returns the tile condition without any changes. Some handlers may implement changes to avoid problematic tiles, improve performance, etc.</remarks>
        protected virtual string ModifyTileCondition(string tileCondition) => tileCondition;

        /// <summary>Places an instance on a tile, if possible.</summary>
        /// <param name="location">The in-game location to use.</param>
        /// <param name="tile">The tile to use.</param>
        /// <param name="instance">The instance to place.</param>
        /// <param name="placementError">Error text describing why an error occured during placement, e.g. an instance's type was invalid. Null if placement succeeeded, or if it only failed due to obstructions.</param>
        /// <returns>True if the instance was successfully spawned. False if spawning was prevented (e.g. by tile obstructions) or if an error occurred.</returns>
        /// <remarks>If <paramref name="placementError"/> is null, a false return value should NOT be treated as an error; it only indicates that placement was skipped. A false return value with non-null error text should be treated as an error (e.g. due to an invalid instance type or caught exception).</remarks>
        protected abstract bool TrySpawn(GameLocation location, Vector2 tile, TSpawn instance, out string placementError);
    }
}
