using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ISpawnItemSettings"/> interface.</summary>
    public static class ISpawnItemSettingsExtensions
    {
        /*********************/
        /* Extension methods */
        /*********************/

        /// <summary>Create items from these settings' item data.</summary>
        /// <param name="queryContext">The game context to use when checking conditions.</param>
        /// <param name="itemContext">The item context to use when generating items.</param>
        /// <param name="timesToRepeat">The number of times to repeat item generation. This may differ from the number of items generated, depending on settings and conditions.</param>
        /// <returns>A set of items generated from these settings' item data. Null entries indicate that an item should be skipped and not spawned.</returns>
        public static IEnumerable<Item> CreateItems<T>(this T settings, GameStateQueryContext queryContext, ItemQueryContext itemContext, int timesToRepeat) where T : ISpawnItemSettings
        {
            if (timesToRepeat <= 0)
                yield break;

            List<FTMSpawnItemData> list = [];
            if (settings.Item != null)
                list.Add(settings.Item);
            if (settings.ItemList != null)
                list.AddRange(settings.ItemList);

            foreach (var entry in list.GetWeightedConditionalElements(settings.ItemListMode, timesToRepeat, queryContext)) //for each item data entry to use
            {
                if (entry.ChanceToSkip > 0 && FTMUtility.Random.NextDouble() < entry.ChanceToSkip)
                    continue;

                //generate one item from query data, if possible
                var item = entry.TryResolveRandomItem(itemContext, inputItem: queryContext.InputItem,
                    logError: (query, error) => FTMUtility.Monitor.Log($"Failed to parse an item query. Context: \"{itemContext.SourcePhrase}\". Query: \"{query}\". Error: \"{error}\".", LogLevel.Warn));

                if (item != null)
                    yield return item;
            }
        }
    }
}
