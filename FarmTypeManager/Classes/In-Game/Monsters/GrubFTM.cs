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
    /// <summary>A subclass of Stardew's Grub class, adjusted for use by this mod.</summary>
    class GrubFTM : Grub
    {
        /// <summary>Creates an instance of Stardew's Grub class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="hard">If true, this grub will be the Mutant Grub subtype.</param>
        public GrubFTM(Vector2 position, bool hard)
            : base(position, hard)
        {
            
        }

        //this override caues GrubFTM to spawn FlyFTM, rather than the base game's Fly
        public override void behaviorAtGameTick(GameTime time)
        {
            base.behaviorAtGameTick(time); //perform Grub's normal method

            //if this spawns a Fly, replace it with a FlyFTM (note: this method is used to avoid overriding the entire method & working around readonly object fields)
            if (Health == -500 && currentLocation.characters[currentLocation.characters.Count - 1] is Fly oldFly)
            {
                FlyFTM newFly = new FlyFTM(oldFly.Position, oldFly.hard); //make a replacement fly of the correct subclass
                newFly.currentLocation = oldFly.currentLocation; //set its current location
                currentLocation.characters[currentLocation.characters.Count - 1] = newFly; //replace the old fly with the new one
            }
        
        }
    }
}
