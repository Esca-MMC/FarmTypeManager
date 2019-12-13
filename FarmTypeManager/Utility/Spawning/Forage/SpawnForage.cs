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
            /// <summary>Generates a object from an index and places it on the specified map and tile.</summary>
            /// <param name="index">The parent sheet index (a.k.a. object ID) of the object type to spawn.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static void SpawnForage(int index, GameLocation location, Vector2 tile)
            {
                StardewValley.Object forageObj = new StardewValley.Object(tile, index, (string)null, false, true, false, true); //generate the forage object
                Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                location.dropObject(forageObj, tile * 64f, Game1.viewport, true, (Farmer)null); //place the forage at the location
            }

            /// <summary>Generates a item from a saved object and places it on the specified map and tile.</summary>
            /// <param name="name">A raw string value describing the item to spawn, e.g. "pizza" or "hat:sombrero".</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static void SpawnForage(SavedObject forage, GameLocation location, Vector2 tile)
            {
                if (forage.Subtype != SavedObject.ObjectSubtype.ForageItem) //if this isn't an "item" subtype
                {
                    SpawnForage(forage.ID.Value, location, tile); //call the object ID version of this method
                    return;
                }

                Item forageItem = null; //the item to be spawned
                string[] categoryAndName = forage.Name.Split(':'); //split the provided name into a category and name

                switch (categoryAndName[0].ToLower()) //based on the category
                {
                    case "bigcraftable":
                    case "bigcraftables":
                    case "big craftable":
                    case "big craftables":
                        forageItem = new StardewValley.Object(tile, forage.ID.Value, false); //create an object as a "big craftable" item
                        break;
                    case "boot":
                    case "boots":
                        forageItem = new Boots(forage.ID.Value);
                        break;
                    case "cloth":
                    case "clothes":
                    case "clothing":
                    case "clothings":
                        forageItem = new Clothing(forage.ID.Value);
                        break;
                    case "furniture":
                        forageItem = new Furniture(forage.ID.Value, tile);
                        break;
                    case "hat":
                    case "hats":
                        forageItem = new Hat(forage.ID.Value);
                        break;
                    case "object":
                    case "objects":
                        SpawnForage(forage.ID.Value, location, tile); //call the object ID version of this method instead
                        return; //end this version of the method
                    case "item":
                    case "items":
                        int stackSize = 1;
                        if (categoryAndName.Length >= 3) //if a stack size was provided (e.g. the forage name is "category:id:stacksize")
                        {
                            int.TryParse(categoryAndName[2], out stackSize); //try to parse the provided stack size
                            stackSize = Math.Max(stackSize, 1); //if stackSize is less than 1 (including if parsing failed), set it to 1
                        }
                        
                        forageItem = new StardewValley.Object(tile, forage.ID.Value, stackSize); //create an object as an item
                        break;
                    case "ring":
                    case "rings":
                        forageItem = new Ring(forage.ID.Value);
                        break;
                    case "weapon":
                    case "weapons":
                        forageItem = new MeleeWeapon(forage.ID.Value);
                        break;
                }

                if (forageItem == null) //if no item could be generated
                {
                    Monitor.Log("The SpawnForage method failed to generate an item. This may be caused by a problem with this mod's logic. Please report this to the developer if possible.", LogLevel.Warn);
                    Monitor.Log($"Item name: {forage.Name}", LogLevel.Warn);
                    Monitor.Log($"Item ID: {forage.ID}", LogLevel.Warn);
                    return;
                }

                Monitor.VerboseLog($"Spawning forage item. Type: {forageItem.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");

                forageItem.ParentSheetIndex = forage.ID.Value; //forcibly set this, due to it being ignored by some item subclasses
                Vector2 pixel = new Vector2((int)tile.X * Game1.tileSize, (int)tile.Y * Game1.tileSize); //get the "pixel" location of the item, rather than the "tile" location
                Debris itemDebris = new Debris(-2, 1, pixel, pixel, 0.1f) //create "debris" to contain the forage item
                {
                    item = forageItem
                };
                itemDebris.Chunks[0].bounces = 3; //prevent the debris bouncing when spawned by increasing its "number of bounces so far" counter
                location.debris.Add(itemDebris); //place the debris at the the location
            }
        }
    }
}