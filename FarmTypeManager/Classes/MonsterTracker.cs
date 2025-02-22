﻿using StardewModdingAPI;
using StardewValley.Monsters;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>A static class that handles monster IDs and certain custom behavior.</summary>
            public static class MonsterTracker
            {
                /// <summary>The highest allowed value of tracked monster IDs.</summary>
                public const int HighestID = -2;

                /// <summary>The next ID to assign to a new monster.</summary>
                /// <remarks>
                /// The base SDV system doesn't seem to pay attention to Monster IDs, and other NPC types generally use positive integers.
                /// As of this mod's design, SDV always assigns the value -1 to monsters. This mod uses values below that to minimize potential issues.
                /// </remarks>
                private static int nextID = HighestID;

                /// <summary>A dictionary of monster instances and sets of saved objects (i.e. "loot" items).</summary>
                private static Dictionary<Monster, IEnumerable<SavedObject>> MonsterLoot = new Dictionary<Monster, IEnumerable<SavedObject>>();

                /// <summary>Clears this class's monster data and resets internal values.</summary>
                public static void Clear()
                {
                    MonsterLoot.Clear();
                    nextID = HighestID;
                }

                /// <summary>Adds a new monster to the tracker and returns an unused ID that may be assigned to it.</summary>
                /// <returns>A unique ID that may be assigned to the monster. Null if the monster could not be added.</returns>
                public static int? AddMonster(Monster monster)
                {
                    if (nextID == int.MinValue) //if there are no more valid IDs left
                        return null;

                    MonsterLoot.Add(monster, new List<SavedObject>(0)); //add the monster with a blank set of loot

                    return nextID--; //return the ID used, then decrement the static field
                }

                /// <summary>Removes the provided monster from the tracker.</summary>
                /// <param name="Monster">The monster instance.</param>
                /// <returns>True if the monster was tracked and has been removed. False if it was not found.</returns>
                public static bool RemoveMonster(Monster monster)
                {
                    if (monster == null)
                        return false;
                    else
                        return MonsterLoot.Remove(monster);
                }

                /// <summary>Assigns a custom loot set to a monster.</summary>
                /// <param name="monster">The monster instance.</param>
                /// <param name="loot">A set of saved objects representing items this monster should drop when defeated.</param>
                public static void SetLoot(Monster monster, IEnumerable<SavedObject> loot)
                {
                    if (MonsterLoot.ContainsKey(monster)) //if the monster exists in the tracker
                        MonsterLoot[monster] = loot;
                    else
                        Monitor.Log($"MonsterTracker error: Attempted to set loot for a monster with an unrecognized ID: {monster?.id}", LogLevel.Debug);
                }

                /// <summary>Get the set of loot assigned to a monster.</summary>
                /// <param name="monster">The monster instance.</param>
                /// <returns>A set of saved objects representing items this monster should drop when defeated. Null if no matching monster exists.</returns>
                public static IEnumerable<SavedObject> GetLoot(Monster monster)
                {
                    if (MonsterLoot.ContainsKey(monster)) //if the monster exists in the tracker
                        return MonsterLoot[monster];
                    else
                        return null;
                }
            }
        }
    }
}