using QuickSave.API;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using FarmTypeManager;

namespace FarmTypeManager.Utilities
{
    /// <summary>Static properties shared throughout this mod, e.g. SMAPI's utility instances.</summary>
    public static class Properties
    {
        /// <summary>The helper provided to this mod by SMAPI.</summary>
        public static IModHelper Helper { get; set; }

        /// <summary>The manifest provided to this mod by SMAPI.</summary>
        public static IManifest Manifest { get; set; }

        /// <summary>The global settings for this mod. Should be set during mod startup.</summary>
        public static ModConfig MConfig { get; set; }

        /// <summary>A shared <see cref="System.Random"/> instance for this mod.</summary>
        public static Random Random { get; } = new Random();

        /******************/
        /* Nested classes */
        /******************/

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

        /// <summary>API instances provided by other mods.</summary>
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

        /// <summary>A set of persistent keys for <see cref="IHaveModData.modData"/> entries.</summary>
        /// <remarks>For legacy reasons, the mod ID is followed by '/' instead of the community-recommended '_'.</remarks>
        public static class ModDataKeys
        {
            /// <summary>The unique key used with the "can be picked up" item setting.</summary>
            public static string CanBePickedUp
            {
                get => field ??= Helper.ModRegistry.ModID + "/CanBePickedUp";
            }

            /// <summary>The unique key used with the "color" monster setting.</summary>
            public static string Color
            {
                get => field ??= Helper.ModRegistry.ModID + "/Color";
            }

            /// <summary>The unique key used with the "disable ranged attacks" monster setting.</summary>
            public static string DisableRangedAttacks
            {
                get => field ??= Helper.ModRegistry.ModID + "/DisableRangedAttacks";
            }

            /// <summary>The unique key used with the "extra loot" monster setting.</summary>
            public static string ExtraLoot
            {
                get => field ??= Helper.ModRegistry.ModID + "/ExtraLoot";
            }

            /// <summary>The unique key used with the "gender" monster setting.</summary>
            public static string Gender
            {
                get => field ??= Helper.ModRegistry.ModID + "/Gender";
            }

            /// <summary>The unique key used with the "instant kill immunity" monster setting.</summary>
            public static string InstantKillImmunity
            {
                get => field ??= Helper.ModRegistry.ModID + "/InstantKillImmunity";
            }

            /// <summary>The unique key used with the "is on" item setting.</summary>
            public static string IsOn
            {
                get => field ??= Helper.ModRegistry.ModID + "/IsOn";
            }

            /// <summary>The unique key used with the "segments" monster setting.</summary>
            public static string Segments
            {
                get => field ??= Helper.ModRegistry.ModID + "/Segments";
            }

            /// <summary>The unique key used to store IDs that identify each instance to its type's serializer.</summary>
            /// <remarks>
            /// <para>
            /// This is only used by instance types that require it.
            /// IDs should be unique among any instances stored in the same set, e.g. all instances at a specific location.
            /// IDs are NOT necessarily globally unique or permanent. For example, a saved instance's ID may change after being loaded.
            /// </para>
            /// </remarks>
            public static string SerializerId
            {
                get => field ??= Helper.ModRegistry.ModID + "/SerializerId";
            }

            /// <summary>The unique key used with the "sight range" monster setting.</summary>
            public static string SightRange
            {
                get => field ??= Helper.ModRegistry.ModID + "/SightRange";
            }

            /// <summary>The unique key used with the "sprite" monster setting.</summary>
            public static string Sprite
            {
                get => field ??= Helper.ModRegistry.ModID + "/Sprite";
            }

            /// <summary>The unique key used to store IDs that spawn instances of a specific type, e.g. a monster type's ID.</summary>
            /// <remarks>
            /// This is not a unique identifier for a single instance; it is similar to <see cref="Item.ItemId"/>. See <see cref="SerializerId"/> to identify instances.
            /// This is only used by certain types that don't provide their own ID-based spawn system (or didn't at the time of writing), e.g. monsters.
            /// </remarks>
            public static string SpawnId
            {
                get => field ??= Helper.ModRegistry.ModID + "/SpawnId";
            }

            /// <summary>The unique key used with the "stun immunity" monster setting.</summary>
            public static string StunImmunity
            {
                get => field ??= Helper.ModRegistry.ModID + "/StunImmunity";
            }
        }

        /// <summary>A set of persistent keys for save data stored with <see cref="IDataHelper.WriteJsonFile"/> or similar methods.</summary>
        /// <remarks>These keys do not use the mod's ID as a prefix. SMAPI's save data system separates each mod's keys.</remarks>
        public static class SaveDataKeys
        {
            /// <summary>The unique key used by <see cref="Serialization.MonsterSerializer"/>.</summary>
            public static string MonsterSerializer => "MonsterSerializer";

            /// <summary>The unique key used by <see cref="Serialization.PlacedItemSerializer"/>.</summary>
            public static string PlacedItemSerializer => "PlacedItemSerializer";
        }
    }
}
