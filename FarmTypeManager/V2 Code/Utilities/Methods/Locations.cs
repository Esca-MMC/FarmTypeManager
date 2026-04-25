using StardewValley;
using StardewValley.Delegates;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.Utilities
{
    /// <summary>Static methods used with <see cref="GameLocation"/> instances and their names.</summary>
    public static class Locations
    {
        /// <summary>Parses a string into a set of recognized location names.</summary>
        /// <param name="locationString">A comma-separated string of location names or other identifiers (see method remarks). Case-insensitive.</param>
        /// <param name="contextLocationName">The unique name of <see cref="GameStateQueryContext.Location"/>. Null if query context is unavailable.</param>
        /// <param name="removeDuplicates">If true, only one of each unique location name may be included in the output. For example, an input string of "Forest, Forest" will output a set of only one string, "Forest".</param>
        /// <returns>A list of <see cref="GameLocation.NameOrUniqueName"/>s for all locations matching the provided string.</returns>
        /// <remarks>
        /// <para>This method will search for location names using the following methods, in order from top to bottom:</para>
        /// <para>Each name may start with one of these prefixes. Currently, prefixes do not include building interior locations.</para>
        /// <list type="bullet">
        ///     <item>
        ///         <term>"Contains:"</term>
        ///         <description>Any non-instanced locations whose names contain the remaining text will be returned. For example, "Contains:arm" will return Farm, Farmhouse, FarmCave, IslandFarmhouse, etc.</description>
        ///     </item>
        ///     <item>
        ///         <term>"Prefix:"</term>
        ///         <description>Any non-instanced locations whose names start with the remaining text will be returned. For example, "Prefix:Farm" will return Farm, Farmhouse, and FarmCave.</description>
        ///     </item>
        ///     <item>
        ///         <term>"Suffix:"</term>
        ///         <description>Any non-instanced locations whose names end with the remaining text will be returned. For example, "Suffix:House" will return Farmhouse, Greenhouse, ScienceHouse, HaleyHouse, etc.</description>
        ///     </item>
        /// </list>
        /// <para>The keyword "<b>Target</b>" refers to <see cref="GameStateQueryContext.Location"/>, e.g. as a trigger action's target location; if unavailable, "Target" is equivalent to "Here".</para>
        /// <para>The keyword "<b>Here</b>" refers to the local player's current location.</para>
        /// <para>If none of the above applies, this method will check for known temporary location names (e.g. UndergroundMine or VolcanoDungeon levels).</para>
        /// <para>Next, it will use <see cref="Game1.getLocationFromName(string)"/> to search for a location with an exact name match.</para>
        /// <para>Next, it will search for any buildings whose IDs match the name (e.g. "Barn") and add all of those buildings' interior locations, if applicable.</para>
        /// </remarks>
        public static List<string> GetLocationNames(string locationString, string contextLocationName = null, bool removeDuplicates = false)
        {
            List<string> locations = [];
            if (locationString == null)
                return locations;

            foreach (string name in locationString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) //split names into separate strings around commas, then check each one
            {
                string[] prefixSplit = name.Split(':', 2); //split this name into prefix and suffix strings, if applicable

                if (prefixSplit.Length == 2) //if this name has a prefix and suffix
                {
                    //handle known prefixes, and skip to the next name afterward
                    switch (prefixSplit[0].ToLower())
                    {
                        case "contains":
                            Utility.ForEachLocation((location) =>
                            {
                                if (location.NameOrUniqueName?.ContainsIgnoreCase(prefixSplit[1]) == true)
                                    locations.Add(location.NameOrUniqueName);
                                return true;
                            }, false, false);
                            continue;

                        case "prefix":
                            Utility.ForEachLocation((location) =>
                            {
                                if (location.NameOrUniqueName?.StartsWithIgnoreCase(prefixSplit[1]) == true)
                                    locations.Add(location.NameOrUniqueName);
                                return true;
                            }, false, false);
                            continue;

                        case "suffix":
                            Utility.ForEachLocation((location) =>
                            {
                                if (location.NameOrUniqueName?.EndsWithIgnoreCase(prefixSplit[1]) == true)
                                    locations.Add(location.NameOrUniqueName);
                                return true;
                            }, false, false);
                            continue;
                    }
                }

                if (name.EqualsIgnoreCase("Target"))
                {
                    locations.Add(contextLocationName ?? Game1.currentLocation.NameOrUniqueName); //mimic GSQ "Target" location: use contextual location if provided, otherwise use local player location
                    continue;
                }

                if (name.EqualsIgnoreCase("Here"))
                {
                    locations.Add(Game1.currentLocation.NameOrUniqueName); //mimic GSQ "Here" location: use local player
                    continue;
                }

                //if this name did not have a prefix OR its prefix was unrecognized, treat it as a normal location name
                if
                (
                    name.StartsWithIgnoreCase("UndergroundMine") //if the name is a mine level (avoid preloading these due to possible errors)
                    || name.StartsWithIgnoreCase("VolcanoDungeon") //or if the name is a volcano level (avoid preloading these due to possible errors)
                    || Game1.getLocationFromName(name) != null //or if the name is a basic, specific location that exists
                )
                {
                    locations.Add(name);
                    continue;
                }

                //if no exact matches were found, try to add any buildings with a matching indoor location
                int buildingsFound = 0;
                Utility.ForEachBuilding((building) =>
                {
                    if (string.Equals(name, building.indoors.Value?.Name, StringComparison.OrdinalIgnoreCase)) //if the indoor Name matches
                    {
                        locations.Add(building.indoors.Value.NameOrUniqueName); //use its unique name
                        buildingsFound++;
                    }
                    return true;
                }, true);
            }

            if (removeDuplicates)
                locations = locations.Distinct().ToList();

            return locations;
        }

        /// <summary>Gets the named location if it's active, i.e. currently loaded and synchronized with the local player.</summary>
        /// <param name="locationName">The name of the location to get. This should match <see cref="GameLocation.NameOrUniqueName"/>.</param>
        /// <returns>The named location instance, or null if it's inactive.</returns>
        public static GameLocation GetLocationIfActive(string locationName)
        {
            if (string.IsNullOrWhiteSpace(locationName))
                return null; //don't bother checking blank names

            GameLocation matchingLocation = null;

            //loop through each location and search for one with a matching name
            Utility.ForEachLocation((location) =>
            {
                if (string.Equals(location?.NameOrUniqueName, locationName, StringComparison.OrdinalIgnoreCase)) //if the location's name matches (case-insensitive)
                {
                    if (location?.IsActiveLocation() == true)
                        matchingLocation = location; //get this location
                    return false; //stop looping
                }
                else
                    return true; //keep looping
            },
            true, true);

            return matchingLocation;
        }
    }
}
