using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Creates a list of all known game location names matching the provided string.</summary>
            /// <param name="locationNames">The name(s) of the location(s) to be listed. Multiple names may be separated by commas. Case-insensitive.</param>
            /// <param name="removeDuplicates">If true, any duplicate names that match exactly will be removed from the final list.</param>
            /// <returns>A list of <see cref="GameLocation.NameOrUniqueName"/>s for all locations matching the provided string.</returns>
            /// <remarks>
            /// <para>Each name in <paramref name="locationNames"/> may start with one of the prefixes below. Currently, prefixes do not include building interior locations.</para>
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
            /// <para>Non-prefixed names will search for exact matches first, then building interiors (matched by building name), then any mod-specific location types.</para>
            /// </remarks>
            public static List<string> GetAllLocationsFromName(string locationNames, bool removeDuplicates = false)
            {
                List<string> locations = [];
                if (locationNames == null) return locations;

                foreach (string name in locationNames.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)) //split names into separate strings around commas, then check each one
                {
                    string[] prefixSplit = name.Split(':', 2); //split this name into prefix and suffix strings, if applicable

                    if (prefixSplit.Length == 2) //if this name has a prefix and suffix
                    {
                        //handle known prefixes, and skip to the next name afterward
                        switch (prefixSplit[0].ToLower())
                        {
                            case "contains":
                                StardewValley.Utility.ForEachLocation((location) =>
                                {
                                    if (location.Name?.ContainsIgnoreCase(prefixSplit[1]) == true)
                                        locations.Add(location.Name);
                                    return true;
                                }, false, false);
                                continue;

                            case "prefix":
                                StardewValley.Utility.ForEachLocation((location) =>
                                {
                                    if (location.Name?.StartsWithIgnoreCase(prefixSplit[1]) == true)
                                        locations.Add(location.Name);
                                    return true;
                                }, false, false);
                                continue;

                            case "suffix":
                                StardewValley.Utility.ForEachLocation((location) =>
                                {
                                    if (location.Name?.EndsWithIgnoreCase(prefixSplit[1]) == true)
                                        locations.Add(location.Name);
                                    return true;
                                }, false, false);
                                continue;
                        }
                    }

                    //if this name did not have a prefix OR its prefix was unrecognized, treat it as a normal location name
                    if
                    (
                        name.StartsWithIgnoreCase("UndergroundMine") //if the name is a mine level (avoid preloading these due to possible errors)
                        || name.StartsWithIgnoreCase("VolcanoDungeon") //or if the name is a volcano level (avoid preloading these due to possible errors)
                        || (Game1.getLocationFromName(name) != null) //or if the name is a basic, specific location that exists
                    )
                    {
                        locations.Add(name);
                        continue;
                    }

                    //if no exact matches were found, try to add any buildings with a matching indoor location
                    int buildingsFound = 0;
                    StardewValley.Utility.ForEachBuilding((building) =>
                    {
                        if (string.Equals(name, building.indoors.Value?.Name, StringComparison.OrdinalIgnoreCase)) //if the indoor Name matches
                        {
                            locations.Add(building.indoors.Value.NameOrUniqueName); //use its unique name
                            buildingsFound++;
                        }
                        return true;
                    }, true);

                    if (buildingsFound > 0)
                        continue;

                    //if all else fails, try to add TMXLoader buildable locations
                    try
                    {
                        if (GetTypeFromName("TMXLoader.TMXLoaderMod") is Type tmx) //if TMXLoader can be accessed
                        {
                            if (tmx.GetField("buildablesBuild", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) is IList tmxSaveBuildables) //if tmx's SaveBuildables list can be accessed
                            {
                                foreach (object sb in tmxSaveBuildables) //for each saved buildable in TMXLoader
                                {
                                    if (sb.GetType() is Type sbType && sbType.GetProperty("UniqueId").GetValue(sb) is string UniqueId && sbType.GetProperty("Id").GetValue(sb) is string Id) //if this buildable's UniqueID and ID can be accessed
                                    {
                                        string mapName = "BuildableIndoors-" + UniqueId; //construct the GameLocation.Name used for this buildable's interior location
                                        if (name == Id && Game1.getLocationFromName(mapName) is GameLocation indoors) //if the provided name equals this buildable's ID AND the interior location exists
                                        {
                                            locations.Add(mapName); //add this location to the list
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Utility.Monitor.LogOnce("Error trying to access TMXLoaderMod class. Skipping building check.", LogLevel.Trace);
                    }
                }

                if (removeDuplicates)
                    locations = locations.Distinct().ToList();

                return locations;
            }
        }
    }
}