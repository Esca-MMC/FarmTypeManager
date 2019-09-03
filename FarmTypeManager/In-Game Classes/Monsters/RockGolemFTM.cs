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
    /// <summary>A subclass of Stardew's RockGolem class, adjusted for use by this mod.</summary>
    class RockGolemFTM : RockGolem
    {
        /// <summary>Creates an instance of Stardew's RockGolem class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="mineLevel">A number that affects the type and/or stats of this monster. This normally represents which floor of the mines the monster spawned on (121+ for skull cavern).</param>
        public RockGolemFTM(Vector2 position, int mineLevel)
            : base(position, mineLevel)
        {

        }
    }
}
