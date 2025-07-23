using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using System;
using System.Collections.Generic;
using static FarmTypeManager.CustomActions.ISpawnItemSettings;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ISpawnItemSettings"/> interface.</summary>
    public static class ISpawnItemSettingsExtensions
    {
        /*********************/
        /* Extension methods */
        /*********************/

        /// <summary>Create items from these settings' item data.</summary>
        /// <param name="gsqContext">The game context to use when checking conditions.</param>
        /// <param name="itemContext">The item context to use when generating items.</param>
        /// <param name="timesToRepeat">The number of times to repeat item generation. This does NOT guarantee the number of items that will be output; some queries may generate multiple items or null items.</param>
        /// <param name="includeNull">If true, null item entries may be included in the return value, e.g. in place of items that were randomly skipped. If false, null entries will be excluded.</param>
        /// <returns>A set of items generated from these settings' item data. Null entries indicate that an item should be skipped and not spawned.</returns>
        public static List<Item> CreateItems<T>(this T settings, GameStateQueryContext gsqContext, ItemQueryContext itemContext, int timesToRepeat, bool includeNull) where T : ISpawnItemSettings
        {
            if (timesToRepeat <= 0)
                return [];

            List<FTMSpawnItemData> data = GetEntries(settings, gsqContext);
            List<Item> items = [];

            switch (settings.ItemListMode)
            {
                case ItemListModes.All:
                    for (int x = 0; x < timesToRepeat; x++)
                    {
                        foreach (var entry in data)
                        {
                            if (FTMUtility.Random.NextDouble() < entry.ChanceToSkip)
                            {
                                if (includeNull)
                                    items.Add(null);
                            }
                            else
                            {
                                //generate one item from query data, if possible
                                var item = entry.TryResolveRandomItem(itemContext, inputItem: gsqContext.InputItem,
                                    logError: (query, error) => FTMUtility.Monitor.Log($"Failed to parse an item query. Context: \"{itemContext.SourcePhrase}\". Query: \"{query}\". Error: \"{error}\".", LogLevel.Warn));

                                if (item != null || includeNull)
                                    items.Add(item);
                            }
                        }
                    }
                    break;
                case ItemListModes.Random:
                default:
                    for (int x = 0; x < timesToRepeat; x++)
                    {
                        var entry = GetWeightedRandom(data);

                        if (FTMUtility.Random.NextDouble() < entry.ChanceToSkip)
                        {
                            if (includeNull)
                                items.Add(null);
                        }
                        else
                        {
                            //generate one item from query data, if possible
                            var item = entry.TryResolveRandomItem(itemContext, inputItem: gsqContext.InputItem,
                                logError: (query, error) => FTMUtility.Monitor.Log($"Failed to parse an item query. Context: \"{itemContext.SourcePhrase}\". Query: \"{query}\". Error: \"{error}\".", LogLevel.Warn));

                            if (item != null || includeNull)
                                items.Add(item);
                        }
                    }
                    break;
            }

            return items;
        }

        /*******************/
        /* Implementations */
        /*******************/

        /// <summary>Get a random entry from a list of item data entries. Each entry's chance to be selected is multiplied by <see cref="FTMSpawnItemData.Weight"/>.</summary>
        /// <param name="list">A list of item data entries.</param>
        /// <returns>A random entry from the list. Each entry's chance to be selected is multiplied by <see cref="FTMSpawnItemData.Weight"/>.</returns>
        private static FTMSpawnItemData GetWeightedRandom(List<FTMSpawnItemData> list)
        {
            int total = 0;
            foreach (var entry in list)
                total += Math.Max(1, entry.Weight);

            int random = FTMUtility.Random.Next(total);
            foreach (var entry in list)
            {
                int weight = Math.Max(1, entry.Weight);
                if (random < weight)
                    return entry;
                else
                    weight -= random;
            }

            throw new Exception("A logic error has caused this randomization method to fail. Please report this to the mod's developer.");
        }

        /// <summary>Get all entries from this instance with valid conditions.</summary>
        /// <param name="context">The game context to use when checking conditions.</param>
        /// <returns>Any entries from this instance with valid conditions.</returns>
        private static List<FTMSpawnItemData> GetEntries<T>(this T settings, GameStateQueryContext context) where T : ISpawnItemSettings
        {
            List<FTMSpawnItemData> entries = [];

            if (settings.Item != null)
                entries.Add(settings.Item);

            if (settings.ItemList != null)
                entries.AddRange(settings.ItemList);

            if (entries.Count == 0)
                return [];

            for (int x = entries.Count - 1; x >= 0; x--) //for each entry, looping backward to allow removal
            {
                if (entries[x].Condition is string condition && GameStateQuery.CheckConditions(condition, context) == false) //if this has a false condition
                    entries.RemoveAt(x);
            }

            return entries;
        }
    }
}
