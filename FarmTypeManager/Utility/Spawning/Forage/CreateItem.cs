using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates an item described by a saved object.</summary>
            /// <param name="save">A saved object with a "category:item name" Name and valid item ID.</param>
            public static Item CreateItem(SavedObject save)
            {
                Item item = null; //the item to be generated

                string[] nameStrings = save.Name.Split(':'); //split the provided name into a category and name

                if (!save.ID.HasValue) //if this save doesn't have an ID
                {
                    GetItemID(nameStrings[0], nameStrings[1]); //generate it
                    if (!save.ID.HasValue) //if this save *still* doesn't have an ID
                    {
                        Monitor.Log("Failed to create an item. Saved object contained no ID and one could not be generated.", LogLevel.Debug);
                        Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                        return null;
                    }
                }

                switch (nameStrings[0].ToLower()) //based on the category
                {
                    case "bigcraftable":
                    case "bigcraftables":
                    case "big craftable":
                    case "big craftables":
                        item = new StardewValley.Object(default(Vector2), save.ID.Value, false); //create an object as a "big craftable" item
                        break;
                    case "boot":
                    case "boots":
                        item = new Boots(save.ID.Value);
                        break;
                    case "cloth":
                    case "clothes":
                    case "clothing":
                    case "clothings":
                        item = new Clothing(save.ID.Value);
                        break;
                    case "furniture":
                        item = new Furniture(save.ID.Value, default(Vector2));
                        break;
                    case "hat":
                    case "hats":
                        item = new Hat(save.ID.Value);
                        break;
                    case "object":
                    case "objects":
                        item = new StardewValley.Object(default(Vector2), save.ID.Value, null, false, true, false, true); //create an object with the preferred constructor for "placed" objects
                        break;
                    case "item":
                    case "items":
                        int stackSize = 1;
                        if (nameStrings.Length >= 3) //if a stack size was provided (e.g. the format is "category:name:stacksize")
                        {
                            int.TryParse(nameStrings[2], out stackSize); //try to parse the provided stack size
                            stackSize = Math.Max(stackSize, 1); //if stackSize is less than 1 (including if parsing failed), set it to 1
                        }

                        item = new StardewValley.Object(default(Vector2), save.ID.Value, stackSize); //create an object with the preferred constructor for "held" or "dropped" items
                        break;
                    case "ring":
                    case "rings":
                        item = new Ring(save.ID.Value);
                        break;
                    case "weapon":
                    case "weapons":
                        item = new MeleeWeapon(save.ID.Value);
                        break;
                }

                if (item == null) //if no item could be generated
                {
                    Monitor.Log("Failed to create an item due to invalid category.", LogLevel.Debug);
                    Monitor.Log($"Item name: {save.Name}", LogLevel.Debug);
                    Monitor.Log($"Item ID: {save.ID}", LogLevel.Debug);
                    return null;
                }

                item.ParentSheetIndex = save.ID.Value; //manually set this, due to it being ignored by some item subclasses

                return item;
            }
        }
    }
}