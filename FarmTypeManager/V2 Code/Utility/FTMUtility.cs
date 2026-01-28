using Microsoft.Xna.Framework;
using QuickSave.API;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace FarmTypeManager
{
    /// <summary>A static class of general utilities for this mod.</summary>
    public static class FTMUtility
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The helper provided to this mod by SMAPI.</summary>
        public static IModHelper Helper { get; set; }

        /// <summary>The manifest provided to this mod by SMAPI.</summary>
        public static IManifest Manifest { get; set; }
        /// <summary>The global settings for this mod. Should be set during mod startup.</summary>
        public static ModConfig MConfig { get; set; }
        /// <summary>A shared <see cref="System.Random"/> instance for this mod.</summary>
        public static Random Random { get; } = new Random();

        /****************************/
        /* Methods - Collision maps */
        /****************************/

        /// <summary>Gets a list of tile coordinates for each "impassable" tile in a collision map, offset relative to the "zero" tile (0,0).</summary>
        /// <param name="collisionMap">A string representing a collision map.</param>
        /// <returns>A list of tile coordinates for each "collision" tile in a collision map, offset relative to the "zero" tile (0,0).</returns>
        /// <remarks>
        /// <para>
        /// A collision map is a string that represents a map of "passable" and "impassable" tiles in a 2D area.
        /// These are partially based on the base game's collision maps, e.g. as used in this field: <see cref="StardewValley.GameData.Buildings.BuildingData.CollisionMap"/>
        /// </para>
        /// <para>
        /// Characters in collision maps are parsed as described below.
        /// If no "zero" tile (0,0) is marked on the map, the top left corner will be used.
        /// Whitespace between lines will be ignored.
        /// Any other unrecognized characters, including interior whitespace, will be treated as passable tiles (e.g. "_" will behave the same way as "O").
        /// <list type="bullet">
        ///     <item>
        ///         <term>X</term>
        ///         <description>An impassable tile, i.e. a tile with collision.</description>
        ///     </item>
        ///     <item>
        ///         <term>O</term>
        ///         <description>A passable tile, i.e. a tile without collision. Default behavior for any unrecognized characters.</description>
        ///     </item>
        ///     <item>
        ///         <term>\n</term>
        ///         <description>A line break symbol. Used to divide tiles onto separate lines; note that normal, in-editor line breaks will also do so. For example, "XXXOOOXXX" is a 1x9 horizontal line. "XXX\nOOO\nXXX" is a 3x3 square.</description>
        ///     </item>
        ///     <item>
        ///         <term>#</term>
        ///         <description>The "zero" tile of this map. Returned tile coordinates are relative to this tile; only one "zero" tile should exist in a map. This symbol makes the tile <b>impassable</b>, i.e. a tile with collision.</description>
        ///     </item>
        ///     <item>
        ///         <term>@</term>
        ///         <description>The "zero" tile of this map. Returned tile coordinates are relative to this tile; only one "zero" tile should exist in a map. This symbol makes the tile <b>passable</b>, i.e. a tile without collision.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        ///     <item>
        ///         <term>XXX</term>
        ///         <description>Returned tiles: 0,0 / 1,0 / 2,0</description>
        ///     </item>
        ///     <item>
        ///         <term>XXX \n OOO \n XXX</term>
        ///         <description>Returned tiles: 0,0 / 1,0 / 2,0 / 0,2 / 1,2 / 2,2</description>
        ///     </item>
        ///     <item>
        ///         <term>XXX \n O@O \n XXX</term>
        ///         <description>Returned tiles: -1,-1 / 0,-1 / 1,-1 / -1,1 / 0,1 / 1,1</description>
        ///     </item>
        ///     <item>
        ///         <term>XXX \n O#O \n XXX</term>
        ///         <description>Returned tiles: -1,-1 / 0,-1 / 1,-1 / 0,0 / -1,1 / 0,1 / 1,1</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static List<Vector2> ParseCollisionMap(string collisionMap)
        {
            if (string.IsNullOrWhiteSpace(collisionMap))
                return [];

            string[] lines = collisionMap.Split('\n', StringSplitOptions.TrimEntries);

            Vector2? zeroTile = null;
            for (int y = 0; y < lines.Length; y++) //for each line
                for (int x = 0; x < lines[y].Length; x++) //for each character in the line
                    if (lines[y][x] is '@' or '#')
                        if (zeroTile == null)
                            zeroTile = new(x, y);
                        else
                            throw new ArgumentException("Collision map contains more than one \"zero tile\" character ('@' or '#').");

            if (zeroTile == null)
                zeroTile = Vector2.Zero;

            List<Vector2> tiles = [];

            for (int y = 0; y < lines.Length; y++) //for each line
                for (int x = 0; x < lines[y].Length; x++) //for each character in the line
                    if (lines[y][x] is 'X' or '#')
                        tiles.Add(new Vector2(x - zeroTile.Value.X, y - zeroTile.Value.Y));

            return tiles;
        }

        /*************************/
        /* Methods - Collections */
        /*************************/

        /// <summary>Randomizes the order of elements in a mutable list.</summary>
        /// <param name="list">The list to randomize.</param>
        public static void RandomizeList<T>(List<T> list)
        {
            for (int index = list.Count - 1; index > 0; index--) //for each index except the first, looping backward
            {
                int random = FTMUtility.Random.Next(index + 1); //get a random index between 0 and this tile's index

                //swap the current element with the element at the random index
                var temp = list[random];
                list[random] = list[index];
                list[index] = temp;
            }
        }

        /// <summary>Yields elements from a list using the specified selection mode.</summary>
        /// <param name="list">The list of elements to use.</param>
        /// <param name="mode">The selection mode to use.</param>
        /// <param name="timesToSelect">The number of elements (or sets of elements) to return, depending on mode.</param>
        /// <returns>A yielded series of elements from the list.</returns>
        public static IEnumerable<T> SelectElementsByMode<T>(List<T> list, SelectionMode mode, int timesToSelect)
        {
            if (list == null || list.Count < 1 || timesToSelect < 1) //if no elements were provided/requested
                yield break; //just return an empty set

            switch (mode)
            {
                case SelectionMode.Random:
                    {
                        for (int yieldCount = 0; yieldCount < timesToSelect; yieldCount++)
                            yield return list[FTMUtility.Random.Next(list.Count)]; //return the requested number of random elements
                        yield break;
                    }
                case SelectionMode.RandomOrder:
                    {
                        int returnCount = 0;
                        while (true)
                        {
                            FTMUtility.RandomizeList(list);
                            for (int index = 0; index < list.Count; index++) //for each element in randomized order
                            {
                                yield return list[index];
                                returnCount++;
                                if (returnCount >= timesToSelect) //if enough elements have been returned
                                    yield break;
                            }
                        }
                    }
                case SelectionMode.Order:
                    {
                        int returnCount = 0;
                        while (true)
                        {
                            for (int index = 0; index < list.Count; index++) //for each element in order
                            {
                                yield return list[index];
                                returnCount++;
                                if (returnCount >= timesToSelect) //if enough elements have been returned
                                    yield break;
                            }
                        }
                    }
                case SelectionMode.ReverseOrder:
                    {
                        int returnCount = 0;
                        while (true)
                        {
                            for (int index = list.Count - 1; index >= 0; index--) //for each element in reverse order
                            {
                                yield return list[index];
                                returnCount++;
                                if (returnCount >= timesToSelect) //if enough elements have been returned
                                    yield break;
                            }
                        }
                    }
                case SelectionMode.All:
                    {
                        for (int x = 0; x < timesToSelect; x++) //repeat the whole process each time
                            for (int index = 0; index < list.Count; index++) //for each element in order
                                yield return list[index];
                        yield break;
                    }
                case SelectionMode.ReverseAll:
                    {
                        for (int x = 0; x < timesToSelect; x++) //repeat the whole process each time
                            for (int index = list.Count - 1; index >= 0; index--) //for each element in reverse order
                                yield return list[index];
                        yield break;
                    }
                default:
                    throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(SelectionMode)); //unrecognized mode value
            }
        }

        /***********************/
        /* Methods - Locations */
        /***********************/

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
                if (string.Equals(location?.NameOrUniqueName, locationName, System.StringComparison.OrdinalIgnoreCase)) //if the location's name matches (case-insensitive)
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

        /*********************/
        /* Methods - Objects */
        /*********************/

        /// <summary>Indicates whether Stardew normally allows a placed object with the given ID to be picked up by players.</summary>
        /// <param name="unqualifiedObjectId">The <see cref="StardewValley.Item.ItemId"/> of a basic non-BC object, without the qualifier "(O)".</param>
        /// <returns>True if Stardew normally allows this object to be picked up.</returns>
        /// <remarks>This checks a hard-coded, manually tested set of values, and may not be accurate for every game/version and/or object.</remarks>
        public static bool CanPickUpByDefault(string unqualifiedObjectId)
        {
            //if this object ID match any known "cannot be picked up" ID, return false; otherwise, return true
            switch (unqualifiedObjectId)
            {
                case "0":   //weeds
                case "2":   //ruby ore
                case "4":   //diamond ore
                case "6":   //jade ore
                case "8":   //amethyst ore
                case "10":  //topaz ore
                case "12":  //emerald ore
                case "14":  //aquamarine ore
                case "25":  //mussel ore
                case "32":  //stone
                case "34":  //
                case "36":  //
                case "38":  //
                case "40":  //
                case "42":  //
                case "44":  //gem ore
                case "46":  //mystic ore
                case "48":  //stone
                case "50":  //
                case "52":  //
                case "54":  //
                case "56":  //
                case "58":  //
                case "75":  //geode ore
                case "76":  //frozen geode ore
                case "77":  //magma geode ore
                case "95":  //radioactive ore
                case "290": //iron ore
                case "294": //twig
                case "295": //
                case "313": //weeds
                case "314": //
                case "315": //
                case "316": //
                case "317": //
                case "318": //
                case "319": //ice crystal (called "weeds" in the object data)
                case "320": //
                case "321": //
                case "343": //stone
                case "450": //
                case "452": //weeds
                case "590": //buried artifact spot
                case "668": //stone
                case "670": //
                case "674": //weeds
                case "675": //
                case "676": //
                case "677": //
                case "678": //
                case "679": //
                case "751": //copper ore
                case "760": //stone
                case "762": //
                case "764": //gold ore
                case "765": //iridium ore
                case "784": //weeds
                case "785": //
                case "786": //
                case "792": //forest farm weed (spring)
                case "793": //forest farm weed (summer)
                case "794": //forest farm weed (fall)
                case "816": //fossil ore
                case "817": //
                case "818": //clay ore
                case "819": //omni geode ore
                case "843": //cinder shard ore
                case "844": //
                case "845": //stone
                case "846": //
                case "847": //
                case "849": //copper ore (volcano/challenge)
                case "850": //iron ore (volcano/challenge)
                case "882": //weeds
                case "883": //
                case "884": //
                case "922": //supply crate (beach farm)
                case "923": //
                case "924": //
                case "BasicCoalNode0":      //coal node
                case "BasicCoalNode1":      //
                case "CalicoEggStone_0":    //calico egg stone (desert festival)
                case "CalicoEggStone_1":    //
                case "CalicoEggStone_2":    //
                case "GreenRainWeeds0":     //green rain weeds
                case "GreenRainWeeds1":     //
                case "GreenRainWeeds2":     //
                case "GreenRainWeeds3":     //
                case "GreenRainWeeds4":     //
                case "GreenRainWeeds5":     //
                case "GreenRainWeeds6":     //
                case "GreenRainWeeds7":     //
                case "VolcanoCoalNode0":    //coal node (volcano/challenge)
                case "VolcanoCoalNode1":    //
                case "VolcanoGoldNode":     //gold node (volcano/challenge)

                    return false; //this ID cannot be picked up

                default:
                    return true; //this ID can be picked up
            }
        }

        /// <summary>Gets the typical health (a.k.a. durability) value used in the game's code for a given <see cref="Object"/> ID, or null if no such value is known.</summary>
        /// <param name="unqualifiedObjectId">The <see cref="StardewValley.Item.ItemId"/> of a basic non-BC object, without the qualifier "(O)".</param>
        /// <returns>The value the game typically uses for this object's <see cref="StardewValley.Object.MinutesUntilReady"/>, also known as health or durability. Null if there is no known default value.</returns>
        /// <remarks>This checks a hard-coded, manually tested set of values, and may not be accurate for every game/version and/or ore type.</remarks>
        public static int? GetDefaultObjectHealth(string unqualifiedObjectId)
        {
            switch (unqualifiedObjectId)
            {
                case "343": //stone (outdoor)
                case "450": //
                    return 1;
                case "32":  //stone (quarry/mine)
                case "38":  //
                case "40":  //
                case "42":  //
                case "668": //
                case "670": //
                    return 2; //varies by context in SDV's code
                case "34": //stone (dark)
                case "36": //
                    return 1;
                case "48": //stone (blue)
                case "50": //
                case "52": //
                case "54": //
                    return 3;
                case "56": //stone (red)
                case "58": //
                    return 4;
                case "845": //stone (volcano)
                case "846": //
                case "847": //
                    return 6;
                case "75": //geode
                    return 3;
                case "76": //frozen geode
                    return 5;
                case "77": //magma geode
                    return 7;
                case "819": //omni geode
                    return 8;
                case "751": //copper
                    return 3;
                case "849": //copper (volcano/challenge)
                    return 6;
                case "290": //iron
                    return 4;
                case "764":             //gold
                case "VolcanoGoldNode": //
                    return 8;
                case "765": //iridium
                    return 16;
                case "46": //mystic
                    return 12;
                case "95": //radioactive
                    return 25;
                case "2":  //diamond
                case "4":  //ruby
                case "6":  //jade
                case "8":  //amethyst
                case "10": //topaz
                case "12": //emerald
                case "14": //aquamarine
                case "44": //gem
                    return 5; //varies by context in SDV's code
                case "25": //mussel
                    return 8;
                case "816": //fossil
                case "817": //
                case "818": //clay
                    return 4;
                case "843": //cinder shard
                case "844": //
                    return 12;
                case "922": //supply crate (beach farm)
                case "923": //
                case "924": //
                    return 3;
                case "BasicCoalNode0": //coal node
                case "BasicCoalNode1": //
                    return 5;
                case "VolcanoCoalNode0": //coal node (volcano/challenge)
                case "VolcanoCoalNode1": //
                    return 10;
                case "CalicoEggStone_0": //calico egg stones (desert festival)
                case "CalicoEggStone_1": //
                case "CalicoEggStone_2": //
                    return 8;

                default: //no known durability
                    return null;
            }
        }

        /***************/
        /* Sub-classes */
        /***************/

        /// <summary>Encapsulates a global <see cref="StardewModdingAPI.IMonitor"/> for this mod. Must be given an IMonitor in the ModEntry class to produce output.</summary>
        public static class Monitor
        {
            private static IMonitor monitor;

            public static IMonitor IMonitor
            {
                set
                {
                    monitor = value;
                }
            }

            /// <summary>True if verbose logging is enabled, i.e. <see cref="VerboseLog(string)"/>.</summary>
            public static bool IsVerbose => monitor.IsVerbose;

            /// <summary>Log a message for the player or developer.</summary>
            /// <param name="message">The message to log.</param>
            /// <param name="level">The log severity level.</param>
            public static void Log(string message, LogLevel level = LogLevel.Debug)
            {
                if (monitor != null)
                {
                    if (MConfig.EnableTraceLogMessages || level != LogLevel.Trace)
                    {
                        monitor.Log(message, level);
                    }
                }
            }

            /// <summary>Log a message for the player or developer, but only if the message has not been logged already this session.</summary>
            /// <param name="message">The message to log.</param>
            /// <param name="level">The log severity level.</param>
            public static void LogOnce(string message, LogLevel level = LogLevel.Debug)
            {
                if (monitor != null) //if the monitor is ready
                {
                    if (MConfig.EnableTraceLogMessages || level != LogLevel.Trace)
                    {
                        monitor.LogOnce(message, level);
                    }
                }
            }

            /// <summary>Log a message that only appears when IMonitor.IsVerbose is enabled.</summary>
            /// <param name="message">The message to log.</param>
            public static void VerboseLog(string message)
            {
                if (monitor != null) //if the monitor is ready
                {
                    if (MConfig.EnableTraceLogMessages)
                    {
                        monitor.VerboseLog(message);
                    }
                }
            }
        }

        /// <summary>A set of persistent keys for <see cref="IHaveModData.modData"/> entries.</summary>
        public static class ModDataKeys
        {
            /// <summary>The unique key used with the <see cref="ConfigItem.CanBePickedUp"/> setting.</summary>
            public static string CanBePickedUp
            {
                get => field ??= FTMUtility.Helper.ModRegistry.ModID + "/CanBePickedUp";
            }

            /// <summary>The unique key used with the "ExtraLoot" setting in <see cref="MonsterType.Settings"/>.</summary>
            public static string ExtraLoot
            {
                get => field ??= FTMUtility.Helper.ModRegistry.ModID + "/ExtraLoot";
            }

            /// <summary>The unique key used with the "InstantKillImmunity" setting in <see cref="MonsterType.Settings"/>.</summary>
            public static string InstantKillImmunity
            {
                get => field ??= FTMUtility.Helper.ModRegistry.ModID + "/InstantKillImmunity";
            }

            /// <summary>The unique key used with the <see cref="FarmTypeManager.CustomActions.FTMSpawnItemData.IsOn"/> setting.</summary>
            public static string IsOn
            {
                get => field ??= FTMUtility.Helper.ModRegistry.ModID + "/IsOn";
            }

            /// <summary>The unique key used with the "StunImmunity" setting in <see cref="MonsterType.Settings"/>.</summary>
            public static string StunImmunity
            {
                get => field ??= FTMUtility.Helper.ModRegistry.ModID + "/StunImmunity";
            }
        }

        /// <summary>Instanced APIs provided by other mods.</summary>
        public static class ModAPIs
        {
            private static bool triedToLoadQuickSaveAPI = false;
            private static IQuickSaveAPI _quickSaveAPI = null;
            public static IQuickSaveAPI QuickSaveAPI
            {
                get
                {
                    if (!triedToLoadQuickSaveAPI)
                    {
                        _quickSaveAPI = Helper.ModRegistry.GetApi<IQuickSaveAPI>("DLX.QuickSave");
                        triedToLoadQuickSaveAPI = true;
                    }

                    return _quickSaveAPI;
                }
            }

            private static bool triedToLoadSaveAnywhereAPI = false;
            private static ISaveAnywhereAPI _saveAnywhereAPI = null;
            public static ISaveAnywhereAPI SaveAnywhereAPI
            {
                get
                {
                    if (!triedToLoadSaveAnywhereAPI)
                    {
                        _saveAnywhereAPI = Helper.ModRegistry.GetApi<ISaveAnywhereAPI>("Omegasis.SaveAnywhere");
                        triedToLoadSaveAnywhereAPI = true;
                    }

                    return _saveAnywhereAPI;
                }
            }
        }
    }
}
