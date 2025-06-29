using StardewModdingAPI;
using StardewValley;
using System;

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

        /***********/
        /* Methods */
        /***********/

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
            private static string _canBePickedUp = null;
            /// <summary>The unique key used with the <see cref="ConfigItem.CanBePickedUp"/> item setting.</summary>
            public static string CanBePickedUp
            {
                get
                {
                    if (_canBePickedUp == null)
                        _canBePickedUp = FTMUtility.Helper.ModRegistry.ModID + "/CanBePickedUp";
                    return _canBePickedUp;
                }
            }

            private static string _extraLoot = null;
            /// <summary>The unique key used with the "ExtraLoot" setting in <see cref="MonsterType.Settings"/>.</summary>
            public static string ExtraLoot
            {
                get
                {
                    if (_extraLoot == null)
                        _extraLoot = FTMUtility.Helper.ModRegistry.ModID + "/ExtraLoot";
                    return _extraLoot;
                }
            }

            private static string _instantKillImmunity = null;
            /// <summary>The unique key used with the "InstantKillImmunity" setting in <see cref="MonsterType.Settings"/>.</summary>
            public static string InstantKillImmunity
            {
                get
                {
                    if (_instantKillImmunity == null)
                        _instantKillImmunity = FTMUtility.Helper.ModRegistry.ModID + "/InstantKillImmunity";
                    return _instantKillImmunity;
                }
            }

            private static string _stunImmunity = null;
            /// <summary>The unique key used with the "StunImmunity" setting in <see cref="MonsterType.Settings"/>.</summary>
            public static string StunImmunity
            {
                get
                {
                    if (_stunImmunity == null)
                        _stunImmunity = FTMUtility.Helper.ModRegistry.ModID + "/StunImmunity";
                    return _stunImmunity;
                }
            }
        }
    }
}
