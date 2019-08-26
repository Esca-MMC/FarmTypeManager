using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            
            public static List<int> GetIDsFromObjects(List<object> objects, SpawnArea area)
            {
                List<int> IDs = new List<int>();

                foreach (object obj in objects)
                {
                    if (obj is long) //if the object is a valid number
                    {
                        IDs.Add(Convert.ToInt32(obj)); //add it to the list as an ID
                    }
                    else if (obj is string name) //if the object is a string (i.e. the name of an item), cast it as a string
                    {
                        bool foundMatchingItem = false;
                        foreach (KeyValuePair<int, string> item in Game1.objectInformation) //for each item in the game's object list
                        {
                            if (name.Trim().Equals(item.Value.Split('/')[0], StringComparison.OrdinalIgnoreCase)) //if the provided name matches this object's name (note: first part of the dictionary value, separated from other settings by '/')
                            {
                                IDs.Add(item.Key); //add the item's ID (which is the dictionary key)
                                foundMatchingItem = true;
                                Monitor.Log($"Index parsed from \"{name}\" into ID: {item.Key}", LogLevel.Trace);
                            }
                        }

                        if (foundMatchingItem == false) //no matching item name could be found
                        {
                            Monitor.Log($"An area's item list contains a name that did not match any items.", LogLevel.Info);
                            Monitor.Log($"Area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Info);
                            Monitor.Log($"Item name: \"{name}\"", LogLevel.Info);
                        }
                    }
                    else //the forage doesn't match any known types
                    {
                        Monitor.Log($"An area's item list contains an unrecognized item format.", LogLevel.Info);
                        Monitor.Log($"Area: \"{area.UniqueAreaID}\" ({area.MapName})", LogLevel.Info);
                        Monitor.Log($"This generally means the list contains a typo. The affected item(s) will be skipped.", LogLevel.Info);
                    }
                }

                return IDs;
            }
        }
    }
}