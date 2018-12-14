using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FarmTypeManager
{
    /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
    static class Utility
    {
        /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on tile index, e.g. tiles using a specific dirt texture).</summary>
        /// <param name="locationName">The name of the GameLocation to check.</param>
        /// <param name="tileIndices">A list of integers representing spritesheet tile indices. Tiles with any matching index will be checked for object spawning.</param>
        /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
        public static List<Vector2> GetTilesByIndex(string locationName, int[] tileIndices)
        {
            GameLocation loc = Game1.getLocationFromName(locationName); //variable for the current location being worked on
            List<Vector2> validTiles = new List<Vector2>(); //will contain x,y coordinates for tiles that are open & valid for new object placement

            //the following loops should populate a list of valid, open tiles for spawning
            int currentTile;
            for (int y = 0; y < (loc.Map.DisplayHeight / Game1.tileSize); y++)
            {
                for (int x = 0; x < (loc.Map.DisplayWidth / Game1.tileSize); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                {
                    currentTile = loc.getTileIndexAt(x, y, "Back"); //get the tile index of the current tile
                    foreach (int index in tileIndices)
                    {
                        if (currentTile == index) //if the current tile matches one of the tile indices
                        {
                            if (loc.isTileLocationTotallyClearAndPlaceable(x, y) == true) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(new Vector2(x, y)); //add to list of valid spawn tiles
                            }
                        }
                    }
                }
            }
            return validTiles;
        }

        /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on tile properties, e.g. the "grass" type).</summary>
        /// <param name="locationName">The name of the GameLocation to check.</param>
        /// <param name="type">A string representing the tile property to match, or a special term used for some additional checks.</param>
        /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
        public static List<Vector2> GetTilesByProperty(string locationName, string type)
        {
            GameLocation loc = Game1.getLocationFromName(locationName); //variable for the current location being worked on
            List<Vector2> validTiles = new List<Vector2>(); //will contain x,y coordinates for tiles that are open & valid for new object placement

            //the following loops should populate a list of valid, open tiles for spawning
            for (int y = 0; y < (loc.Map.DisplayHeight / Game1.tileSize); y++)
            {
                for (int x = 0; x < (loc.Map.DisplayWidth / Game1.tileSize); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                {
                    if (type.Equals("all", StringComparison.OrdinalIgnoreCase)) //if the "property" to be matched is "All" (a special exception)
                    {
                        //add any clear tiles, regardless of properties
                        if (loc.isTileLocationTotallyClearAndPlaceable(x, y) == true) //if the tile is clear of any obstructions
                        {
                            validTiles.Add(new Vector2(x, y)); //add to list of valid spawn tiles
                        }
                    }
                    if (type.Equals("diggable", StringComparison.OrdinalIgnoreCase)) //if the tile's "Diggable" property matches (case-insensitive)
                    {
                        if (loc.doesTileHaveProperty(x, y, "Diggable", "Back") == "T") //NOTE: the string "T" means "true" for several tile property checks
                        {
                            if (loc.isTileLocationTotallyClearAndPlaceable(x, y) == true) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(new Vector2(x, y)); //add to list of valid spawn tiles
                            }
                        }
                    }
                    else //assumed to be checking for a specific value in the tile's "Type" property, e.g. "Grass" or "Dirt"
                    {
                        string currentType = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? ""; //NOTE: this sets itself to a blank (not null) string to avoid null errors when comparing it

                        if (currentType.Equals(type, StringComparison.OrdinalIgnoreCase)) //if the tile's "Type" property matches (case-insensitive)
                        {
                            if (loc.isTileLocationTotallyClearAndPlaceable(x, y) == true) //if the tile is clear of any obstructions
                            {
                                validTiles.Add(new Vector2(x, y)); //add to list of valid spawn tiles
                            }
                        }
                    }
                }
            }
            return validTiles;
        }

        /// <summary>Produces a list of x/y coordinates for valid, open tiles for object spawning at a location (based on a string describing two vectors).</summary>
        /// <param name="locationName">The name of the GameLocation to check.</param>
        /// <param name="vectorString">A string describing two vectors. Parsed into vectors and used to find a rectangular area.</param>
        /// <returns>A list of Vector2, each representing a valid, open tile for object spawning at the given location.</returns>
        public static List<Vector2> GetTilesByVectorString(string locationName, string vectorString)
        {
            GameLocation loc = Game1.getLocationFromName(locationName); //variable for the current location being worked on
            List<Vector2> validTiles = new List<Vector2>(); //x,y coordinates for tiles that are open & valid for new object placement
            List<Tuple<Vector2, Vector2>> vectorPairs = new List<Tuple<Vector2, Vector2>>(); //pairs of x,y coordinates representing areas on the map (to be scanned for valid tiles)

            //parse the "raw" string representing two coordinates into actual numbers, populating "vectorPairs"
            string[] xyxy = vectorString.Split(new char[] { ',', '/', ';' }); //split the string into separate strings based on various delimiter symbols
            if (xyxy.Length != 4) //if "xyxy" didn't split into the right number of strings, it's probably formatted poorly
            {
                Monitor.Log($"This include/exclude area in your player file isn't formatted correctly: \"{vectorString}\" ({locationName})", LogLevel.Info);
            }
            else
            {
                int[] numbers = new int[4]; //this section will convert "xyxy" into four numbers and store them here
                bool success = true;
                for (int i = 0; i < 4; i++)
                {
                    if (Int32.TryParse(xyxy[i].Trim(), out numbers[i]) != true) //attempts to store each "xyxy" string as an integer in "numbers"; returns false if it failed
                    {
                        success = false;
                    }
                }

                if (success) //everything was successfully parsed, apparently
                {
                    //convert the numbers to a pair of vectors and add them to the list
                    vectorPairs.Add(new Tuple<Vector2, Vector2>(new Vector2(numbers[0], numbers[1]), new Vector2(numbers[2], numbers[3])));
                }
                else
                {
                    Monitor.Log($"This include/exclude area in your player file isn't formatted correctly: \"{vectorString}\" ({locationName})", LogLevel.Info);
                }
            }

            //check the area marked by "vectorPairs" for valid, open tiles and populate "validTiles" with them
            foreach (Tuple<Vector2, Vector2> area in vectorPairs)
            {
                for (int y = (int)Math.Min(area.Item1.Y, area.Item2.Y); y <= (int)Math.Max(area.Item1.Y, area.Item2.Y); y++) //use the lower Y first, then the higher Y; should define the area regardless of which corners/order the user wrote down
                {
                    for (int x = (int)Math.Min(area.Item1.X, area.Item2.X); x <= (int)Math.Max(area.Item1.X, area.Item2.X); x++) //loops for each tile on the map, from the top left (x,y == 0,0) to bottom right, moving horizontally first
                    {
                        if (loc.isTileLocationTotallyClearAndPlaceable(x, y) == true) //if the tile is clear of any obstructions
                        {
                            validTiles.Add(new Vector2(x, y)); //add to list of valid spawn tiles
                        }
                    }
                }
            }

            return validTiles;
        }

        ///<summary>Safely encapsulates IMonitor.Log() for this mod's static classes. Must be given an IMonitor in the ModEntry class to produce output.</summary>
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

            ///<summary>Log a message for the player or developer.</summary>
            /// <param name="message">The message to log.</param>
            /// <param name="level">The log severity level.</param>
            public static void Log(string message, LogLevel level = LogLevel.Debug)
            {
                if (monitor != null)
                {
                    monitor.Log(message, level);
                }
            }
        }
    }
}
