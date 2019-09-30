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
using Microsoft.Xna.Framework.Graphics;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's BigSlime class, adjusted for use by this mod.</summary>
    class BigSlimeFTM : BigSlime
    {
        public int MineLevelOfDeathSpawns { get; set; } = 0; //determines the subtype of any monsters spawned when this monster dies

        /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="mineLevel">A number that affects the type and/or stats of this monster. This normally represents which floor of the mines the monster spawned on (121+ for skull cavern).</param>
        public BigSlimeFTM(Vector2 position, int mineLevel)
            : base(position, mineLevel)
        {
            MineLevelOfDeathSpawns = mineLevel;
        }

        //this override fixes the following BigSlime behavioral bugs:
        // * small slimes not spawning when the big slime dies
        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int num1 = Math.Max(1, damage - resilience.Value);
            if (Game1.random.NextDouble() < missChance.Value - missChance.Value * addedPrecision)
            {
                num1 = -1;
            }
            else
            {
                this.Slipperiness = 3;
                this.Health -= num1;
                this.setTrajectory(xTrajectory, yTrajectory);
                this.currentLocation.playSound("hitEnemy");
                this.IsWalkingTowardPlayer = true;
                if (this.Health <= 0)
                {
                    this.deathAnimation();
                    ++Game1.stats.SlimesKilled;
                    if (Game1.gameMode == (byte)3 && Game1.random.NextDouble() < 0.75)
                    {
                        int num2 = Game1.random.Next(2, 5);
                        for (int index = 0; index < num2; ++index)
                        {
                            this.currentLocation.characters.Add((NPC)new GreenSlime(this.Position, MineLevelOfDeathSpawns)); //use MineLevelOfDeathSpawns instead of checking the game state
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].setTrajectory(xTrajectory / 8 + Game1.random.Next(-2, 3), yTrajectory / 8 + Game1.random.Next(-2, 3));
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].willDestroyObjectsUnderfoot = false;
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].moveTowardPlayer(4);
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].Scale = (float)(0.75 + (double)Game1.random.Next(-5, 10) / 100.0);
                            this.currentLocation.characters[this.currentLocation.characters.Count - 1].currentLocation = this.currentLocation;
                        }
                    }
                }
            }
            return num1;
        }
    }
}
