using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of criteria for <see cref="Monster"/> instances, used to filter or compare them.</summary>
    public class MonsterMatchData
    {
        /// <summary>If true, these criteria will be inverted, producing the opposite result when checked. Any instances that did NOT match will match instead, and vice versa.</summary>
        public bool InvertResults { get; set; } = false;

        /**********************/
        /* Monster properties */
        /**********************/

        public string SpawnId { get; set; } = null;
        public List<string> SpawnIdList { get; set; } = null;

        public string Name { get; set; } = null;
        public List<string> NameList { get; set; } = null;

        public int? MinCurrentHealth { get; set; } = null;
        public int? MaxCurrentHealth { get; set; } = null;

        public float? MinCurrentHealthPercent { get; set; } = null;
        public float? MaxCurrentHealthPercent { get; set; } = null;

        public Dictionary<string, string> ModData { get; set; } = null;

        /***********/
        /* Methods */
        /***********/

        /// <summary>Checks whether this data matches the given monster.</summary>
        /// <param name="monster">The monster to check.</param>
        /// <returns>True if the monster matches this data's criteria. False if the monster doesn't match.</returns>
        public bool Match(Monster monster)
        {
            //NOTE: Return data.InvertResults instead of false, and !data.InvertResults instead of true. When that setting is true, it should correctly invert the results.

            if (SpawnId != null)
                if (!monster.modData.TryGetValue(Utilities.Properties.ModDataKeys.SpawnId, out string spawnIdValue) || !SpawnId.Equals(spawnIdValue, StringComparison.OrdinalIgnoreCase))
                    return InvertResults;

            if (SpawnIdList != null)
                if (!monster.modData.TryGetValue(Utilities.Properties.ModDataKeys.SpawnId, out string spawnIdValue) || !SpawnIdList.Contains(spawnIdValue, StringComparer.OrdinalIgnoreCase))
                    return InvertResults;

            if (Name != null && !Name.Equals(monster.Name, StringComparison.OrdinalIgnoreCase))
                return InvertResults;

            if (NameList != null && !NameList.Contains(monster.Name, StringComparer.OrdinalIgnoreCase))
                return InvertResults;

            if (MinCurrentHealth.HasValue && monster.Health < MinCurrentHealth.Value)
                return InvertResults;

            if (MaxCurrentHealth.HasValue && monster.Health > MaxCurrentHealth.Value)
                return InvertResults;

            if (MinCurrentHealthPercent.HasValue && (float)monster.Health / monster.MaxHealth * 100 < MinCurrentHealthPercent.Value) //multiply by 100 to use actual percents, e.g. 50 = 50%
                return InvertResults;

            if (MaxCurrentHealthPercent.HasValue && (float)monster.Health / monster.MaxHealth * 100 > MaxCurrentHealthPercent.Value)
                return InvertResults;

            if (ModData != null)
            {
                foreach (var entry in ModData)
                {
                    //NOTE: Keys with null values should be considered equal to keys that don't exist.
                    //      For example, if this match data specifies {"Key1": null}, and the instance's data has {"Key1": null} OR doesn't have a "Key1" entry at all, then it matches.

                    monster.modData.TryGetValue(entry.Key, out string instanceValue); //get this entry from the instance's mod data, or null if it doesn't exist
                    if (!string.Equals(entry.Value, instanceValue, StringComparison.OrdinalIgnoreCase))
                        return InvertResults;
                }
            }

            return !InvertResults; //everything matched
        }
    }
}
