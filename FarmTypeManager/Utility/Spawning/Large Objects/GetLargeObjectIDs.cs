using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.GiantCrops;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates a list of IDs for large objects (a.k.a. resource clumps) from a list of IDs and/or nicknames. Duplicates are kept; invalid entries are removed.</summary>
            /// <param name="names">A list of names representing large objects (e.g. "Stump", "boulders", and/or specific IDs).</param>
            /// /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A list of valid large object IDs parsed from the provided list.</returns>
            public static List<string> GetLargeObjectIDs(string[] names, string areaID = "")
            {
                List<string> IDs = new List<string>(); //a list of index numbers to be returned

                foreach (string name in names)
                {
                    //for each valid name, add the game's internal ID for that large object (a.k.a. resource clump)
                    switch (name.ToLower())
                    {
                        case "600":
                        case "stump":
                        case "stumps":
                            IDs.Add("600");
                            break;
                        case "602":
                        case "log":
                        case "logs":
                            IDs.Add("602");
                            break;
                        case "672":
                        case "boulder":
                        case "boulders":
                            IDs.Add("672");
                            break;
                        case "622":
                        case "meteor":
                        case "meteors":
                        case "meteorite":
                        case "meteorites":
                            IDs.Add("622");
                            break;
                        case "752":
                        case "minerock1":
                        case "mine rock 1":
                            IDs.Add("752");
                            break;
                        case "minerock2":
                        case "mine rock 2":
                            IDs.Add("754");
                            break;
                        case "756":
                        case "minerock3":
                        case "mine rock 3":
                            IDs.Add("756");
                            break;
                        case "758":
                        case "minerock4":
                        case "mine rock 4":
                            IDs.Add("758");
                            break;
                        case "190":
                        case "cauliflower":
                        case "giantcauliflower":
                        case "giant cauliflower":
                            IDs.Add("190");
                            break;
                        case "254":
                        case "melon":
                        case "giantmelon":
                        case "giant melon":
                            IDs.Add("254");
                            break;
                        case "276":
                        case "pumpkin":
                        case "giantpumpkin":
                        case "giant pumpkin":
                            IDs.Add("276");
                            break;
                        default: //if "name" isn't a known object name or ID
                            var giantCropsData = Game1.content.Load<Dictionary<string, GiantCropData>>("Data\\GiantCrops");
                            if (giantCropsData.ContainsKey(name)) //if this is a giant crop ID
                                IDs.Add(name);
                            else
                            {
                                Monitor.Log($"An area's large object list contains a name that did not match any objects.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                                Monitor.Log($"Object name: \"{name}\"", LogLevel.Info);
                            }
                            break;
                    }
                }

                return IDs;
            }
        }
    }
}