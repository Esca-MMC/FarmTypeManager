using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace FarmTypeManager
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Console command. Outputs the player's current location name, tile x/y coordinates, tile "Type" property (e.g. "Grass" or "Dirt"), tile "Diggable" status, and tile index.</summary>
        private void WhereAmI(string command, string[] args)
        {
            if (!Context.IsWorldReady) { return; } //if the player isn't in a fully loaded game yet, ignore this command

            GameLocation loc = Game1.currentLocation;
            Point tile = Game1.player.TilePoint;
            int index = loc.getTileIndexAt(tile.X, tile.Y, "Back");
            string type = loc.doesTileHaveProperty(tile.X, tile.Y, "Type", "Back") ?? "[none]";
            string diggable = loc.doesTileHaveProperty(tile.X, tile.Y, "Diggable", "Back");
            if (diggable != null) { diggable = "Yes"; } else { diggable = "No"; }

            Utility.Monitor.Log($"Map name: {loc.Name}", LogLevel.Info);
            Utility.Monitor.Log($"Your location (x,y): {tile.X},{tile.Y}", LogLevel.Info);
            Utility.Monitor.Log($"Terrain type: {type}", LogLevel.Info);
            Utility.Monitor.Log($"Diggable: {diggable}", LogLevel.Info);
            Utility.Monitor.Log($"Tile image index: {index}", LogLevel.Info);
        }
    }
}