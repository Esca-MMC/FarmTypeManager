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
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {
        ///<summary>Tasks performed when the mod initially loads.</summary>
        public override void Entry(IModHelper helper)
        {
            ModConfig conf = helper.ReadConfig<ModConfig>(); //create or load the config.json file
            Utility.Monitor.IMonitor = Monitor; //pass the monitor for use by Utility methods and other detached code sections

            if (conf.EnableWhereAmICommand == true) //if enabled, add the WhereAmI method as a console command
            {
                helper.ConsoleCommands.Add("whereami", "Outputs the (x,y) coordinates and tile index value for the player's current location.", WhereAmI);
            }

            Helper.Events.GameLoop.DayStarted += DayStarted;
        }

        /// <summary>Tasks performed after the game begins a new day, including when loading a save.</summary>
        private void DayStarted(object sender, EventArgs e)
        {
            if (Context.IsMainPlayer != true) { return; } //if the player using this mod is a multiplayer farmhand, don't try to do anything

            Utility.Config = Helper.Data.ReadJsonFile<FarmConfig>($"data/{Constants.SaveFolderName}.json"); //load the current save's config file ([null] if it doesn't exist)
            if (Utility.Config == null) //no config file for this save?
            {
                Utility.Config = new FarmConfig(); //load the default config settings
                Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", Utility.Config); //create a config file for the current save
            }

            //run the various main processes
            ObjectSpawner.ForageGeneration();
            ObjectSpawner.HardwoodGeneration();
            ObjectSpawner.OreGeneration();
        }

        ///<summary>Console command. Outputs the player's current location name, tile x/y coordinates, tile index, and tile "Type" property (e.g. "Grass" or "Dirt").</summary>
        private void WhereAmI(string command, string[] args)
        {
            string locName = Game1.currentLocation.Name;
            int x = Game1.player.getTileX();
            int y = Game1.player.getTileY();
            int index = Game1.currentLocation.getTileIndexAt(x, y, "Back");
            string type = Game1.currentLocation.doesTileHaveProperty(x, y, "Type", "Back") ?? "[none]";
            Monitor.Log($"Tile (x,y): {x},{y}. Map name: {locName}", LogLevel.Info);
            Monitor.Log($"Tile index: {index}. Terrain type: {type}", LogLevel.Info);
        }
    }
}