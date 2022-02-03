using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's LavaLurk class, adjusted for use by this mod.</summary>
    public class LavaLurkFTM : LavaLurk
    {
        /// <summary>True if this monster's normal ranged attack behavior should be enabled.</summary>
        public bool RangedAttacks { get; set; } = true;

        /// <summary>Creates an instance of Stardew's LavaLurk class, but with adjustments made for this mod.</summary>
        public LavaLurkFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's LavaLurk class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="rangedAttacks">True if this monster's normal ranged attack behavior should be enabled.</param>
        public LavaLurkFTM(Vector2 position, bool rangedAttacks = true)
            : base(position)
        {
            RangedAttacks = rangedAttacks;
        }

        public override void behaviorAtGameTick(GameTime time)
        {
            base.behaviorAtGameTick(time); //run the original method for this monster
            if (!RangedAttacks) //if ranged attacks are disabled
            {
                fireTimer = int.MaxValue; //set fireball cooldown to max
            }
        }
    }
}
