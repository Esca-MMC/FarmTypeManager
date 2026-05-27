using StardewValley;
using StardewValley.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of criteria for <see cref="Item"/> instances, used to filter or compare them.</summary>
    public class ItemMatchData
    {
        /// <summary>If true, these criteria will be inverted, producing the opposite result when checked. Any instances that did NOT match will match instead, and vice versa.</summary>
        public bool InvertResults { get; set; } = false;

        /*******************/
        /* Item properties */
        /*******************/

        public string Id { get; set; } = null;
        public List<string> IdList { get; set; } = null;

        public string Name { get; set; } = null;
        public List<string> NameList { get; set; } = null;

        public string Type { get; set; } = null;
        public List<string> TypeList { get; set; } = null;

        public int? Category { get; set; } = null;
        public List<int> CategoryList { get; set; } = null;

        public string ContextTag { get; set; } = null;
        public List<string> ContextTagList { get; set; } = null;

        public int? MinQuality { get; set; } = null;
        public int? MaxQuality { get; set; } = null;

        public int? MinStackSize { get; set; } = null;
        public int? MaxStackSize { get; set; } = null;

        public Dictionary<string, string> ModData { get; set; } = null;

        public string PerItemCondition { get; set; } = null;

        /*********************/
        /* Object properties */
        /*********************/

        public int? Fragility { get; set; } = null;

        public int? MinMinutesUntilReady { get; set; } = null;
        public int? MaxMinutesUntilReady { get; set; } = null;

        /***********/
        /* Methods */
        /***********/

        /// <summary>Checks whether this data matches the given item.</summary>
        /// <param name="item">The item to check.</param>
        /// <param name="location">The location to use as context, overriding query context. If null, the original context will be used.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <returns>True if the item matches this data's criteria. False if the item doesn't match.</returns>
        public bool Match(Item item, GameLocation location, GameStateQueryContext queryContext)
        {
            //NOTE: Return data.InvertResults instead of false, and !data.InvertResults instead of true. When that setting is true, it should correctly invert the results.

            //item properties

            if (Id != null && !string.Equals(item.QualifiedItemId, Id, StringComparison.OrdinalIgnoreCase))
                return InvertResults;

            if (IdList != null && !IdList.Contains(item.QualifiedItemId, StringComparer.OrdinalIgnoreCase))
                return InvertResults;

            if (Name != null && !string.Equals(Name, item.Name, StringComparison.OrdinalIgnoreCase))
                return InvertResults;

            if (NameList != null && !NameList.Contains(item.Name, StringComparer.OrdinalIgnoreCase))
                return InvertResults;

            if (Type != null && !string.Equals(Type, item.GetItemTypeId(), StringComparison.OrdinalIgnoreCase))
                return InvertResults;

            if (TypeList != null && !TypeList.Contains(item.GetItemTypeId(), StringComparer.OrdinalIgnoreCase))
                return InvertResults;

            if (Category != null && Category != item.Category)
                return InvertResults;

            if (CategoryList != null && !CategoryList.Contains(item.Category))
                return InvertResults;

            if (ContextTag != null)
            {
                if (!ItemContextTagManager.DoesTagQueryMatch(ContextTag, item.GetContextTags())) //if the item is missing any context tags (note: method supports multiple comma-separated tags and '!' negation)
                    return InvertResults;
            }

            if (ContextTagList != null && ContextTagList.Count > 0)
            {
                bool anyStringMatches = false;
                var itemTags = item.GetContextTags();

                foreach (string tagString in ContextTagList)
                {
                    if (ItemContextTagManager.DoesTagQueryMatch(tagString, itemTags)) //if the item's tags match this string
                    {
                        anyStringMatches = true;
                        break;
                    }
                }

                if (!anyStringMatches)
                    return InvertResults;
            }

            if (MinQuality != null && MinQuality > item.Quality)
                return InvertResults;

            if (MaxQuality != null && MaxQuality < item.Quality)
                return InvertResults;

            if (MinStackSize != null && MinStackSize > item.Stack)
                return InvertResults;

            if (MaxStackSize != null && MaxStackSize < item.Stack)
                return InvertResults;

            if (ModData != null)
            {
                foreach (var entry in ModData)
                {
                    //NOTE: Keys with null values should be considered equal to keys that don't exist.
                    //      For example, if this match data specifies {"Key1": null}, and the instance's data has {"Key1": null} OR doesn't have a "Key1" entry at all, then it matches.

                    item.modData.TryGetValue(entry.Key, out string instanceValue); //get this entry from the instance's mod data, or null if it doesn't exist
                    if (!string.Equals(entry.Value, instanceValue, StringComparison.OrdinalIgnoreCase))
                        return InvertResults;
                }
            }

            if (PerItemCondition != null && !GameStateQuery.CheckConditions(PerItemCondition, location ?? queryContext.Location, queryContext.Player, item)) //if the provided condition doesn't match (using the most relevant/available context data)
                return InvertResults;

            //object properties

            var obj = item as Object;

            if (Fragility != null && (obj == null || obj.Fragility != Fragility))
                return InvertResults;

            if (MinMinutesUntilReady != null && (obj == null || MinMinutesUntilReady > obj.MinutesUntilReady))
                return InvertResults;

            if (MaxMinutesUntilReady != null && (obj == null || MaxMinutesUntilReady < obj.MinutesUntilReady))
                return InvertResults;

            return !InvertResults; //everything matched
        }
    }
}
