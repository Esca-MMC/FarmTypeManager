using StardewValley;
using StardewValley.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ItemMatchData"/> class.</summary>
    public static class ItemMatchDataExtensions
    {
        /// <summary>Checks whether this data matches the given item.</summary>
        /// <param name="data">The match data to check.</param>
        /// <param name="item">The item to check.</param>
        /// <param name="location">The location to use as context, overriding query context. If null, the original context will be used.</param>
        /// <param name="queryContext">Contextual information to use when checking conditions.</param>
        /// <returns>True if the item matches this data's criteria. False if the item doesn't match.</returns>
        public static bool Match(this ItemMatchData data, Item item, GameLocation location, GameStateQueryContext queryContext)
        {
            if (data.Category != null && data.Category != item.Category)
                return false;

            if (data.CategoryList != null && !data.CategoryList.Contains(item.Category))
                return false;

            if (data.ContextTag != null)
            {
                if (!ItemContextTagManager.DoesTagQueryMatch(data.ContextTag, item.GetContextTags())) //if the item is missing any context tags (note: method supports multiple comma-separated tags and '!' negation)
                    return false;
            }

            if (data.ContextTagList != null && data.ContextTagList.Count > 0)
            {
                bool anyStringMatches = false;
                var itemTags = item.GetContextTags();

                foreach (string tagString in data.ContextTagList)
                {
                    if (ItemContextTagManager.DoesTagQueryMatch(tagString, itemTags)) //if the item's tags match this string
                    {
                        anyStringMatches = true;
                        break;
                    }
                }

                if (!anyStringMatches)
                    return false;
            }

            if (data.Id != null && !string.Equals(item.QualifiedItemId, data.Id, StringComparison.OrdinalIgnoreCase))
                return false;

            if (data.IdList != null && !data.IdList.Contains(item.QualifiedItemId, StringComparer.OrdinalIgnoreCase))
                return false;

            if (data.MinQuality != null && data.MinQuality > item.Quality)
                return false;

            if (data.MaxQuality != null && data.MaxQuality < item.Quality)
                return false;

            if (data.MinStackSize != null && data.MinStackSize > item.Stack)
                return false;

            if (data.MaxStackSize != null && data.MaxStackSize < item.Stack)
                return false;

            if (data.ModData != null)
            {
                foreach (var entry in data.ModData)
                {
                    if (!item.modData.TryGetValue(entry.Key, out string value)) //if the item doesn't have the specified key
                        return false;
                    if (value != null && !string.Equals(entry.Value, value, StringComparison.OrdinalIgnoreCase)) //if the specified value isn't null and doesn't match the item value
                        return false;
                }
            }

            if (data.Name != null && !string.Equals(data.Name, item.Name, StringComparison.OrdinalIgnoreCase))
                return false;

            if (data.NameList != null && !data.NameList.Contains(item.Name, StringComparer.OrdinalIgnoreCase))
                return false;

            if (data.PerItemCondition != null && !GameStateQuery.CheckConditions(data.PerItemCondition, location ?? queryContext.Location, queryContext.Player, item)) //if the provided condition doesn't match (using the most relevant/available context data)
                return false;

            if (data.Type != null && !string.Equals(data.Type, item.GetItemTypeId(), StringComparison.OrdinalIgnoreCase))
                return false;

            if (data.TypeList != null && !data.TypeList.Contains(item.GetItemTypeId(), StringComparer.OrdinalIgnoreCase))
                return false;

            //check fields that are specific to StardewValley.Object

            var obj = item as Object;

            if (data.Fragility != null && (obj == null || obj.Fragility != data.Fragility))
                return false;

            if (data.MinMinutesUntilReady != null && (obj == null || data.MinMinutesUntilReady > obj.MinutesUntilReady))
                return false;

            if (data.MaxMinutesUntilReady != null && (obj == null || data.MaxMinutesUntilReady < obj.MinutesUntilReady))
                return false;

            //everything matches

            return true; 
        }
    }
}
