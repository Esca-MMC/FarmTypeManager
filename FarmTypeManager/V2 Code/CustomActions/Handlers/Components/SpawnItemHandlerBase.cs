using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System.Collections.Generic;
using xTile.Dimensions;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A base for handlers that spawn one or more <see cref="Item"/> instances on tiles.</summary>
    public abstract class SpawnItemHandlerBase<TSettings> : SpawnHandlerBase<TSettings, Item>, ICustomActionHandler
        where TSettings : class, ILocationSettings, ITimesToPerformSettings, ITileSettings, ISpawnItemSettings
    {
        /********************/
        /* SpawnHandlerBase */
        /********************/

        protected override IEnumerable<Item> CreateInstances(TSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfTimes)
        {
            ItemQueryContext itemContext = new(queryContext.Location, queryContext.Player, queryContext.Random, $"FTM custom actions > SpawnItemHandlerBase item creation");
            return settings.CreateItems(queryContext, itemContext, numberOfTimes);
        }

        protected override string GetId(Item instance) => instance?.QualifiedItemId;
    }
}
