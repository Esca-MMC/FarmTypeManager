using StardewModdingAPI;
using StardewValley;
using System;

namespace FarmTypeManager
{
    /// <summary>A static class of general utilities for this mod.</summary>
    public static class FTMUtility
    {
        /**********/
        /* Fields */
        /**********/

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
    }
}
