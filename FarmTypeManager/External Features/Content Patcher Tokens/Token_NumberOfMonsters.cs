using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.ExternalFeatures.ContentPatcherTokens
{
    /// <summary>A Content Patcher token that indicates how many monsters are in a named location.</summary>
    public class Token_NumberOfMonsters
    {
        /* Private fields */

        /// <summary>A set of token inputs and outputs for the most recent context update.</summary>
        /// <remarks>The output may depend on the current local player, which varies in split-screen mode.</remarks>
        private PerScreen<Dictionary<string, int>> InputOutputCache { get; set; } = new PerScreen<Dictionary<string, int>>(() => new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

        /* Public methods */

        /** Metadata methods **/

        /// <summary>Get whether the token allows input arguments (e.g. an NPC name for a relationship token).</summary>
        public bool AllowsInput()
        {
            return true;
        }

        /// <summary>Whether the token requires input arguments to work, and does not provide values without it (see <see cref="AllowsInput"/>).</summary>
        public bool RequiresInput()
        {
            return false;
        }

        /// <summary>Whether the token may return multiple values for the given input.</summary>
        /// <param name="input">The input arguments, if applicable.</param>
        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        /// <summary>Get whether the token always returns a value within a bounded numeric range for the given input. Mutually exclusive with <see cref="HasBoundedValues"/>.</summary>
        /// <param name="input">The input arguments, if any.</param>
        /// <param name="min">The minimum value this token may return.</param>
        /// <param name="max">The maximum value this token may return.</param>
        /// <remarks>Default false.</remarks>
        public bool HasBoundedRangeValues(string input, out int min, out int max)
        {
            min = -1; //special value for locations that currently aren't loaded/synced
            max = int.MaxValue;
            return true;
        }

        /** State methods **/

        /// <summary>Update the values when the context changes.</summary>
        /// <returns>Returns whether the value changed, which may trigger patch updates.</returns>
        public bool UpdateContext()
        {
            InputOutputCache.Value.Clear(); //clear the local player's cache

            Utility.ForEachLocation((location) => //for each location the local player has loaded (not necessarily active/synced)
            {
                if (location.IsActiveLocation()) //if this location is active, i.e. the local player has accurate info about NPCs there
                {
                    int monsterCount = 0;
                    foreach (NPC npc in location.characters)
                    {
                        if (npc is Monster)
                            monsterCount++;
                    }
                    InputOutputCache.Value[location.NameOrUniqueName] = monsterCount; //update cache for this location
                }
                return true; //iterate to the next location
            }, true, true); //include interiors and temp maps

            return true; //assume values changed (TODO: compare to previous cache, return false if equal)
        }

        /// <summary>Get whether the token is available for use.</summary>
        public bool IsReady()
        {
            return Context.IsWorldReady;
        }

        /// <summary>Get the current values.</summary>
        /// <param name="input">The input arguments, if any.</param>
        public IEnumerable<string> GetValues(string input)
        {
            string locationName;

            if (string.IsNullOrWhiteSpace(input)) //if no input was provided
                locationName = Game1.player.currentLocation?.NameOrUniqueName ?? ""; //use the local player's current location
            else
                locationName = input;

            if (InputOutputCache.Value.TryGetValue(locationName, out int monsterCount))
                yield return monsterCount.ToString();
            else
                yield return "-1"; //special value for locations that currently aren't loaded/synced
        }
    }
}