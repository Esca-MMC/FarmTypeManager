﻿using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

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

                if (rawItems == null) return SavedObjects;

                foreach (object raw in rawItems) //for each object in the raw list
                {
                    if (raw is long rawLong) //if this is an integer
                    {
                        SavedObject saved = CreateSavedObject(rawLong.ToString(), areaID); //treat this as a string ID, try to create a saved object
                        if (saved != null)
                            SavedObjects.Add(saved);
                    }
                    else if (raw is string rawString) //if this is a string
                    {
                        SavedObject saved = CreateSavedObject(rawString, areaID); //try to create a saved object
                        if (saved != null)
                            SavedObjects.Add(saved);

                    }
                    else if (raw is JObject rawObj) //if this is a ConfigItem or SavedObject
                    {
                        SavedObject saved = null;
                        ConfigItem item = null;
                        try
                        {
                            if (rawObj.ContainsKey("ConfigItem")) //if this contains the "ConfigItem" key, it should already be a SavedObject
                            {
                                saved = rawObj.ToObject<SavedObject>(); //parse this as a SavedObject
                            }
                            else
                            {
                                item = rawObj.ToObject<ConfigItem>(); //parse this as a ConfigItem
                                saved = CreateSavedObject(item, areaID); //use it to create a saved object
                            }
                        }
                        catch
                        {
                            Monitor.Log($"An area's item list contains a complex item that could not be parsed properly.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                            Monitor.Log($"This may be caused by a formatting error in the item list. The affected item will be skipped.", LogLevel.Info);
                        }

                        if (saved != null) //if parsing was successful
                        {
                            SavedObjects.Add(saved); //add this to the list
                        }
                    }
                    else //the object doesn't match any known types
                    {
                        Monitor.Log($"An area's item list contains an unrecognized item format.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                        Monitor.Log($"This may be caused by a formatting error in the item list. The affected item will be skipped.", LogLevel.Info);
                    }
                }

                return SavedObjects;
            }

            /// <summary>Uses a string to create a saved object representing a StardewValley.Object.</summary>
            /// <param name="objectName">The name of the object.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A saved object representing the designated StardewValley.Object. Null if creation failed.</returns>
            private static SavedObject CreateSavedObject(string objectName, string areaID = "")
            {
                string objectID = GetItemID("object", objectName); //get an object ID for this name

                if (objectID != null) //if a matching object ID was found
                {
                    SavedObject saved = new SavedObject() //generate a saved object with the appropriate type, ID, and name
                    {
                        Type = SavedObject.ObjectType.Object,
                        Name = objectName,
                        StringID = objectID,
                        ConfigItem = new ConfigItem() { Category = "object" }
                    };
                    Monitor.VerboseLog($"Parsed \"{objectName}\" into object ID: {objectID}");
                    return saved;
                }
                else //if no matching object ID was found
                {
                    Monitor.Log($"An area's item list contains an object ID or name that did not match any loaded objects.", LogLevel.Info);
                    Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                    Monitor.Log($"Object name: \"{objectName}\"", LogLevel.Info);
                    Monitor.Log($"This may be caused by an error in the item list or a modded object that wasn't loaded. The affected object will be skipped.", LogLevel.Info);
                    return null;
                }
            }

            /// <summary>Uses a ConfigItem to create a saved object representing an item.</summary>
            /// <param name="item">The ConfigItem class describing the item.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A saved object representing the designated item. Null if creation failed.</returns>
            private static SavedObject CreateSavedObject(ConfigItem item, string areaID = "")
            {
                switch (item.Type)
                {
                    case SavedObject.ObjectType.Object:
                    case SavedObject.ObjectType.Item:
                    case SavedObject.ObjectType.Container:
                    case SavedObject.ObjectType.DGA:
                        //these are valid item types
                        break;
                    default:
                        Monitor.Log($"An area's item list contains a complex item with a type that is not recognized.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                        Monitor.Log($"Item type: \"{item.Type}\"", LogLevel.Info);
                        Monitor.Log($"This is likely due to a design error in the mod's code. Please report this to the mod's developer. The affected item will be skipped.", LogLevel.Info);
                        return null;
                }

                if (item.Contents != null) //if this item has contents
                {
                    for (int x = item.Contents.Count - 1; x >= 0; x--) //for each of the contents
                    {
                        List<SavedObject> contentSave = ParseSavedObjectsFromItemList(new object[] { item.Contents[x] }, areaID); //attempt to parse this into a saved object
                        if (contentSave.Count <= 0) //if parsing failed
                        {
                            item.Contents.RemoveAt(x); //remove this from the contents list
                        }
                    }
                }

                if (item.Type == SavedObject.ObjectType.Container) //if this is a container
                {
                    //containers have no name or ID to validate, so don't involve them

                    SavedObject saved = new SavedObject() //generate a saved object with these settings
                    {
                        Type = item.Type,
                        ConfigItem = item
                    };
                    Monitor.VerboseLog($"Parsed \"{item.Category}\" as a container type.");
                    return saved;
                }

                if (item.Type == SavedObject.ObjectType.DGA) //if this is a DGA item
                {
                    if (DGAItemAPI != null) //if DGA's API is loaded
                    {
                        try
                        {
                            object testItem = DGAItemAPI.SpawnDGAItem(item.Name); //confirm that this item can be created

                            if (testItem != null) //if this item was created successfully
                            {
                                if (testItem is Item) //if the item is an Item or any subclass of it (SDV object, etc)
                                {
                                    SavedObject saved = new SavedObject() //generate a saved object with these settings
                                    {
                                        Type = item.Type,
                                        Name = item.Name,
                                        ConfigItem = item
                                    };
                                    Monitor.VerboseLog($"Parsed \"{item.Name}\" as a DGA item.");
                                    return saved;
                                }
                                else //if this item not an Item
                                {
                                    Monitor.Log($"An area's item list contains a Dynamic Game Assets (DGA) item of a type that FTM does not recognize.", LogLevel.Info);
                                    Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                                    Monitor.Log($"Item name: \"{item.Name}\"", LogLevel.Info);
                                    Monitor.Log($"This may be caused by an error in the item list or a type of custom item that FTM cannot spawn. The affected item will be skipped.", LogLevel.Info);
                                    return null;
                                }
                            }
                            else
                            {
                                Monitor.Log($"An area's item list contains a Dynamic Game Assets (DGA) item name that does not match any loaded DGA items.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                                Monitor.Log($"Item name: \"{item.Name}\"", LogLevel.Info);
                                Monitor.Log($"This may be caused by an error in the item list or a modded object that wasn't loaded. The affected item will be skipped.", LogLevel.Info);
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Monitor.Log($"An area's item list contains a Dynamic Game Assets (DGA) item, but an error occurred while test-spawning the item.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                            Monitor.Log($"Item name: \"{item.Name}\"", LogLevel.Info);
                            Monitor.Log($"The affected item will be skipped. The auto-generated error message has been added to the log.", LogLevel.Info);
                            Monitor.Log($"----------", LogLevel.Trace);
                            Monitor.Log($"{ex.ToString()}", LogLevel.Trace);
                            return null;
                        }
                    }
                    else //if DGA's API is unavailable
                    {
                        Monitor.Log($"An area's item list contains a Dynamic Game Assets (DGA) item, but that mod's interface is unavailable.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                        Monitor.Log($"Item name: \"{item.Name}\"", LogLevel.Info);
                        Monitor.Log($"If DGA is not installed, please install it. If FTM displayed an error about DGA's interface, please report this to FTM's developer. The affected item will be skipped.", LogLevel.Info);
                        return null;
                    }
                }

                string savedName = item.Category + ":" + item.Name;

                string itemID = GetItemID(item.Category, item.Name); //get an item ID for the category and name
                if (itemID != null) //if a matching item ID was found
                {
                    SavedObject saved = new SavedObject() //generate a saved object with these settings
                    {
                        Type = item.Type,
                        Name = savedName,
                        StringID = itemID,
                        ConfigItem = item
                    };
                    Monitor.VerboseLog($"Parsed \"{item.Category}\": \"{item.Name}\" into item ID: {itemID}");
                    return saved;
                }
                else //if no matching item ID was found
                {
                    Monitor.Log($"An area's item list contains a complex item definition that did not match any loaded items.", LogLevel.Info);
                    Monitor.Log($"Affected spawn area: \"{areaID}\"", LogLevel.Info);
                    Monitor.Log($"Item name: \"{savedName}\"", LogLevel.Info);
                    Monitor.Log($"This may be caused by an error in the item list or a modded item that wasn't loaded. The affected item will be skipped.", LogLevel.Info);
                    return null;
                }
            }
        }
    }
}