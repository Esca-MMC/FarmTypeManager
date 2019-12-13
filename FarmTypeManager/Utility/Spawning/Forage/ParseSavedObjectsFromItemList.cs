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
            /// <summary>Parses a "raw" list of Items into a new list of SavedObjects, excluding any that aren't successfully parsed.</summary>
            /// <param name="rawObjects">A list of objects, each describing a specific "kind" of Item.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A list of SavedObjects representing each successfully parsed item.</returns>
            public static List<SavedObject> ParseSavedObjectsFromItemList(IEnumerable<object> rawItems, string areaID = "")
            {
                List<SavedObject> SavedObjects = new List<SavedObject>();

                foreach (object raw in rawItems) //for each object in the raw list
                {
                    if (raw is long) //if this is the ID of a StardewValley.Object
                    {
                        int ID = Convert.ToInt32(raw); //parse it as an object ID
                        SavedObject saved = new SavedObject(null, default(Vector2), SavedObject.ObjectType.Forage, ID, null, null); //generate a saved object with the appropriate type and ID
                        SavedObjects.Add(saved); //add it to the list
                        Monitor.VerboseLog($"Parsed integer object ID: {ID}");
                    }
                    else if (raw is string rawString)
                    {
                        if (!rawString.Contains(':')) //if this is the name of a StardewValley.Object
                        {
                            int? ID = GetItemID("object", rawString); //get an object ID for this name

                            if (ID.HasValue) //if a matching object ID was found
                            {
                                SavedObject saved = new SavedObject(null, default(Vector2), SavedObject.ObjectType.Forage, ID, rawString, null); //generate a saved object with the appropriate type, ID, and name
                                SavedObjects.Add(saved); //add it to the list
                                Monitor.VerboseLog($"Parsed \"{rawString}\" into object ID: {ID}");
                            }
                            else //if no matching object ID was found
                            {
                                Monitor.Log($"An area's item list contains an object name that did not match any loaded objects.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                                Monitor.Log($"Item name: \"{rawString}\"", LogLevel.Info);
                                Monitor.Log($"This may be caused by an error in the item list or a modded object that wasn't loaded. The affected object(s) will be skipped.", LogLevel.Info);
                            }
                        }
                        else //if this is a "category:name" description
                        {
                            string[] catAndName = rawString.Split(':'); //split the string, resulting in a "category" and "name" (among other possible strings)
                            int? ID = GetItemID(catAndName[0], catAndName[1]); //get an item ID for the category and name
                            if (ID.HasValue) //if a matching item ID was found
                            {
                                SavedObject saved = new SavedObject(null, default(Vector2), SavedObject.ObjectType.Forage, ID, rawString, null); //generate a saved object with the appropriate type, ID, and name
                                if (!catAndName[0].Equals("object", StringComparison.OrdinalIgnoreCase) && !catAndName[0].Equals("objects", StringComparison.OrdinalIgnoreCase)) //if this isn't a StardewValley.Object
                                {
                                    saved.Subtype = SavedObject.ObjectSubtype.ForageItem; //mark this as an Item
                                }
                                SavedObjects.Add(saved); //add it to the list
                                Monitor.VerboseLog($"Parsed \"{rawString}\" into item ID: {ID}");
                            }
                            else //if no matching item ID was found
                            {
                                Monitor.Log($"An area's item list contains an item name that did not match any loaded items.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                                Monitor.Log($"Item name: \"{rawString}\"", LogLevel.Info);
                                Monitor.Log($"This may be caused by an error in the item list or a modded item that wasn't loaded. The affected item(s) will be skipped.", LogLevel.Info);
                            }
                        }
                    }
                    else //the object doesn't match any known types
                    {
                        Monitor.Log($"An area's item list contains an unrecognized item format.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                        Monitor.Log($"This may be caused by a formatting error in the item list. The affected item(s) will be skipped.", LogLevel.Info);
                    }
                }

                return SavedObjects;
            }
        }
    }
}