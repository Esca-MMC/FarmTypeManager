using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates a large object and places it on the specified map and tile.</summary>
            /// <param name="index">The parent sheet index (a.k.a. object ID) of the object type to spawn.</param>
            /// <param name="location">The GameLocation where the large object should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the ore should be spawned.</param>
            public static bool SpawnLargeObject(string index, GameLocation location, Vector2 tile)
            {
                Monitor.VerboseLog($"Spawning large object. ID: {index}. Location: {tile.X},{tile.Y} ({location.Name}).");

                switch (index)
                {
                    //if this is a known resource clump ID
                    case "600":
                    case "602":
                    case "622":
                    case "672":
                    case "752":
                    case "754":
                    case "756":
                    case "758":
                        location.resourceClumps.Add(new ResourceClump(int.Parse(index), 2, 2, tile));
                        return true;
                    default: //if this is NOT a known clump, assume it's a giant crop
                        location.resourceClumps.Add(new GiantCrop(index.ToString(), tile));
                        return true;
                }
            }
        }
    }
}