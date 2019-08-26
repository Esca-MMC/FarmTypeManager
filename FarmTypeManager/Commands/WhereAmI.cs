using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;

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
            int x = Game1.player.getTileX();
            int y = Game1.player.getTileY();
            int index = loc.getTileIndexAt(x, y, "Back");
            string type = loc.doesTileHaveProperty(x, y, "Type", "Back") ?? "[none]";
            string diggable = loc.doesTileHaveProperty(x, y, "Diggable", "Back");
            if (diggable == "T") { diggable = "Yes"; } else { diggable = "No"; };
            Monitor.Log($"Map name: {loc.Name}", LogLevel.Info);
			Monitor.Log($"Your location (x,y): {x},{y}", LogLevel.Info);
            Monitor.Log($"Terrain type: {type}", LogLevel.Info);
            Monitor.Log($"Diggable: {diggable}", LogLevel.Info);
            Monitor.Log($"Tile image index: {index}", LogLevel.Info);

            //TODO TEMP TEST ETC
            //testing stuff below here
            if (args.Length > 0)
            {
                Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                if (args.Length > 1)
                    dict.Add("minelevel", Convert.ToInt32(args[1]));
                else
                    dict.Add("minelevel", 0);

                dict.Add("mincolor", "0 0 0");
                dict.Add("maxcolor", "255 255 255");

                List<object> loot = new List<object>();
                loot.Add((long)420); //be careful with this elsewhere, since apparently SMAPI's JSON conversion just happens to read numbers as longs
                loot.Add("pizza");

                dict.Add("Loot", loot);

                MonsterType monster = new MonsterType(args[0], dict);

                Utility.SpawnMonster(monster, loc, new Vector2(x, y - 1), new SpawnArea());
            }
        }
    }
}