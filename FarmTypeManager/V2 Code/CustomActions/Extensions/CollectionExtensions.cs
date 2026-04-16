using FarmTypeManager.Utilities;
using StardewValley;
using StardewValley.Delegates;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for generic collection types.</summary>
    public static class CollectionExtensions
    {
        /// <summary>Yields a series of key-value pairs from a dictionary, after checking the values' conditions and adjusting for weight.</summary>
        /// <typeparam name="TKey">The dictionary's key type.</typeparam>
        /// <typeparam name="TValue">The dictionary's value type. Must implement <see cref="IWeightedConditionalElement"/>.</typeparam>
        /// <param name="dictionary">The dictionary to use.</param>
        /// <param name="mode">The mode to use while selecting elements to return.</param>
        /// <param name="timesToSelect">The number of key-value pairs (or sets of key-value pairs) to return, depending on mode.</param>
        /// <param name="queryContext">The game state query (GSQ) context to use when checking conditions.</param>
        /// <returns>A yielded series of key-value pairs from the dictionary.</returns>
        public static IEnumerable<KeyValuePair<TKey, TValue>> GetWeightedConditionalElements<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, SelectionMode mode, int timesToSelect, GameStateQueryContext queryContext)
            where TValue : IWeightedConditionalElement
        {
            if (dictionary == null || dictionary.Count < 1 || timesToSelect < 1) //if no pairs were provided/requested
                yield break; //just return an empty set

            List<KeyValuePair<TKey, TValue>> parsedList = []; //create a list of active pairs, including duplicates based on weight
            foreach (var pair in dictionary)
            {
                if (pair.Value == null
                   || pair.Value.Weight <= 0
                   || (pair.Value.MarkAppliedWithFlag != null && Game1.player.hasOrWillReceiveMail(pair.Value.MarkAppliedWithFlag)) //if this is marked as applied
                   || (!string.IsNullOrWhiteSpace(pair.Value.Condition) && !GameStateQuery.CheckConditions(pair.Value.Condition, queryContext))) //if the condition is false
                    continue;

                for (int x = 0; x < pair.Value.Weight; x++)
                    parsedList.Add(pair); //add copies of this pair equal to the value's weight
            }

            if (parsedList.Count < 1) //if no pairs were active
                yield break;

            foreach (var output in Collections.SelectElementsByMode(parsedList, mode, timesToSelect)) //yield pairs using the given mode
            {
                if (output.Value.MarkAppliedWithFlag != null)
                    Game1.player.mailReceived.Add(output.Value.MarkAppliedWithFlag); //add this entry's flag
                yield return output;
            }
        }

        /// <summary>Yields a series of elements from a list, after checking the elements' conditions and adjusting for weight.</summary>
        /// <param name="list">The list to use.</param>
        /// <param name="mode">The mode to use while selecting elements to return.</param>
        /// <param name="timesToSelect">The number of elements (or sets of elements) to return, depending on mode.</param>
        /// <param name="queryContext">The game state query (GSQ) context to use when checking conditions.</param>
        /// <returns>A yielded series of elements from the list.</returns>
        public static IEnumerable<T> GetWeightedConditionalElements<T>(this List<T> list, SelectionMode mode, int timesToSelect, GameStateQueryContext queryContext)
            where T : IWeightedConditionalElement
        {
            if (list == null || list.Count < 1 || timesToSelect < 1) //if no elements were provided/requested
                yield break; //just return an empty set

            List<T> parsedList = []; //create a list of active elements, including duplicates based on weight
            foreach (var element in list)
            {
                if (element == null
                   || element.Weight <= 0
                   || (element.MarkAppliedWithFlag != null && Game1.player.hasOrWillReceiveMail(element.MarkAppliedWithFlag)) //if this is marked as applied
                   || (!string.IsNullOrWhiteSpace(element.Condition) && !GameStateQuery.CheckConditions(element.Condition, queryContext))) //if the condition is false
                    continue;

                for (int x = 0; x < element.Weight; x++)
                    parsedList.Add(element); //add copies of this element equal to its weight
            }

            if (parsedList.Count < 1) //if no elements were active
                yield break;

            foreach (var output in Collections.SelectElementsByMode(parsedList, mode, timesToSelect)) //yield elements using the given mode
            {
                if (output.MarkAppliedWithFlag != null)
                    Game1.player.mailReceived.Add(output.MarkAppliedWithFlag); //add this entry's flag
                yield return output;
            }
        }
    }
}
