using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using Newtonsoft.Json;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Creates a list of all game locations, including building interiors, matching the provided name.</summary>
            /// <param name="name">The name of the location(s) to be listed. Case-insensitive.</param>
            /// <returns>A list of all locations with a Name property matching </returns>
            public static List<GameLocation> GetAllLocationsFromName(string name)
            {
                List<GameLocation> locations; //the final list of matching locations
                
                GameLocation location = Game1.getLocationFromName(name); //attempt to get a "static" map location with the provided name
                if (location != null) //if a location was found
                {
                    locations = new List<GameLocation> { location }; //create a list containing the location
                }
                else //if a location was not found
                {
                    locations = new List<GameLocation>(); //create a blank list

                    foreach (BuildableGameLocation buildable in Game1.locations.OfType<BuildableGameLocation>()) //for each buildable location in the game
                    {
                        foreach (Building building in buildable.buildings.Where(building => building.indoors.Value != null)) //for each building with an interior location ("indoors")
                        {
                            if (building.indoors.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) //if the location's name matches the provided name
                            {
                                locations.Add(building.indoors.Value); //add the location to the list
                            }
                        }
                    }
                }

                return locations;
            }
        }
    }
}