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
            public static bool SpawnForage(int index, GameLocation location, Vector2 tile)
            {
                StardewValley.Object forageObj;

                if (CanBePickedUp(index)) //if this object can be picked up
                {
                    forageObj = new StardewValley.Object(tile, index, null, false, true, false, true); //generate the forage object
                    Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    return location.dropObject(forageObj, tile * 64f, Game1.viewport, true, null); //attempt to place the object and return success/failure
                }
                else //if this object CANNOT be picked up
                {
                    forageObj = new StardewValley.Object(tile, index, 1); //use an alternative constructor
                    Monitor.VerboseLog($"Spawning forage object. Type: {forageObj.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    location.objects.Add(tile, forageObj); //add the object directly to the objects list
                    return true;
                }
            }

            /// <summary>Generates a item from a saved object and places it on the specified map and tile.</summary>
            /// <param name="forage">The SavedObject containing this forage's information.</param>
            /// <param name="location">The GameLocation where the forage should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnForage(SavedObject forage, GameLocation location, Vector2 tile)
            {
                if (forage.Type == SavedObject.ObjectType.Object) //if this is a basic object
                {
                    return SpawnForage(forage.ID.Value, location, tile); //call the object ID version of this method
                }
                else if (forage.Type == SavedObject.ObjectType.Container) //if this is a container
                {
                    Item container = CreateItem(forage, tile); //create the container to be spawned

                    if (container == null || !(container is StardewValley.Object)) //if the container couldn't be created or isn't a StardewValley.Object
                    {
                        Monitor.Log("The SpawnForage method failed to generate a container. This may be caused by a problem with this mod's logic. Please report this to the developer if possible.", LogLevel.Warn);
                        Monitor.Log($"Container type: {forage.Name}", LogLevel.Warn);
                        Monitor.Log($"Item ID: {forage.ID}", LogLevel.Warn);
                        return false;
                    }

                    if (location.objects.ContainsKey(tile)) //if this tile is already occupied in the object dictionary
                    {
                        Monitor.VerboseLog("Tile is already occupied by an object. Skipping container spawn.");
                    }

                    Monitor.VerboseLog($"Spawning container. Type: {container.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    location.objects.Add(tile, (StardewValley.Object)container); //add the container to the location's object array
                    return true;
                }
                else //if this is an item
                {
                    if (location.terrainFeatures.ContainsKey(tile)) //if a terrain feature already exists on this tile
                        return false; //fail to spawn

                    Item forageItem = CreateItem(forage, tile); //create the item to be spawned

                    if (forageItem == null) //if the item couldn't be created
                    {
                        Monitor.Log("The SpawnForage method failed to generate an item. This may be caused by a problem with this mod's logic. Please report this to the developer if possible.", LogLevel.Warn);
                        Monitor.Log($"Item name: {forage.Name}", LogLevel.Warn);
                        Monitor.Log($"Item ID: {forage.ID}", LogLevel.Warn);
                        return false;
                    }

                    Monitor.VerboseLog($"Spawning forage item. Type: {forageItem.DisplayName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                    PlacedItem placed = new PlacedItem(tile, forageItem); //create a terrainfeature containing the item
                    location.terrainFeatures.Add(tile, placed); //add the placed item to this location
                    return true;
                }
            }

            /// <summary>Indicates whether SDV normally allows a placed object with the given ID to be picked up.</summary>
            /// <param name="objectID">The object's ID, a.k.a. parent sheet index.</param>
            /// <returns>True if SDV normally allows this object to be picked up.</returns>
            private static bool CanBePickedUp(int objectID)
            {
                //if this object ID match any known "cannot be picked up" ID, return false; otherwise, return true
                switch (objectID)
                {
                    case 0:   //weeds
                    case 2:   //ruby ore
                    case 4:   //diamond ore
                    case 6:   //jade ore
                    case 8:   //amethyst ore
                    case 10:  //topaz ore
                    case 12:  //emerald ore
                    case 14:  //aquamarine ore
                    case 25:  //mussel ore
                    case 44:  //gem ore
                    case 46:  //mystic ore
                    case 75:  //geode ore
                    case 76:  //frozen geode ore
                    case 77:  //magma geode ore
                    case 95:  //radioactive ore
                    case 290: //iron ore
                    case 294: //twig
                    case 295: //
                    case 313: //weeds
                    case 314: //
                    case 315: //
                    case 316: //
                    case 317: //
                    case 318: //
                    case 319: //ice crystal (called "weeds" in the object data)
                    case 320: //
                    case 321: //
                    case 343: //stone
                    case 450: //
                    case 452: //weeds
                    case 590: //buried artifact spot
                    case 668: //stone
                    case 670: //
                    case 674: //weeds
                    case 675: //
                    case 676: //
                    case 677: //
                    case 678: //
                    case 679: //
                    case 751: //copper ore
                    case 760: //stone
                    case 762: //
                    case 764: //gold ore
                    case 765: //iridium ore
                    case 784: //weeds
                    case 785: //
                    case 786: //
                    case 792: //forest farm weed (spring)
                    case 793: //forest farm weed (summer)
                    case 794: //forest farm weed (fall)
                    case 816: //fossil ore
                    case 817: //
                    case 818: //clay ore
                    case 819: //omni geode ore
                    case 843: //cinder shard ore
                    case 844: //
                    case 845: //stone
                    case 846: //
                    case 847: //
                    case 849: //copper ore (volcano)
                    case 850: //iron ore (volcano)
                    case 882: //weeds
                    case 883: //
                    case 884: //
                        return false; //this ID cannot be picked up
                    default:
                        return true; //this ID can be picked up
                }
            }
        }
    }
}