using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's RockGolem class, adjusted for use by this mod.</summary>
    public class RockGolemFTM : RockGolem
    {
        /// <summary>Creates an instance of Stardew's RockGolem class, but with adjustments made for this mod.</summary>
        public RockGolemFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's RockGolem class (Stone Golem subtype), but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public RockGolemFTM(Vector2 position)
            : base(position)
        {
            //immediately set the golem to its "hiding" state, fixing a bug when spawned near players
            Sprite.currentFrame = 16;
            Sprite.loop = false;
            Sprite.UpdateSourceRect();
        }

        /// <summary>Creates an instance of Stardew's RockGolem class (Wilderness/Iridium Golem subtypes), but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="difficultyMod">The difficulty modifier to use. Affects health, damage, experience gained, loot items, and subtype (see remarks).</param>
        /// <param name="forceIridiumGolem">If true, an iridium golem will be generated. Otherwise, the result will unmodified from the original constructor (see remarks).</param>
        /// <remarks>
        /// As of SDV v1.6, if difficultyMod is 9 or higher and the player has the Wilderness farm type, there is a 50% chance that the base constructor (Vector2, int) will create an "Iridium Golem". Otherwise, it will create a "Wilderness Golem".
        /// </remarks>
        public RockGolemFTM(Vector2 position, int difficultyMod, bool forceIridiumGolem = false)
            : base(position, difficultyMod)
        {
            if (forceIridiumGolem && Name != "Iridium Golem") //if this should be an iridium golem, but the base constructor generated a wilderness golem
            {
                //imitate the "correct" behavior that should've occurred in the Character and NPC constructors
                Sprite = new AnimatedSprite("Characters\\Monsters\\Iridium Golem");
                Name = "Iridium Golem";
                //NOTE: the private field "originalSourceRect" should normally be set to "sprite.SourceRect", but the values should match between sprites anyway

                parseMonsterInfo(Name); //reload monster info (stats, display name, etc)

                //run the original constructor's code
                base.IsWalkingTowardPlayer = false;
                base.Slipperiness = 3;
                base.HideShadow = true;
                jitteriness.Value = 0.0;
                base.DamageToFarmer += difficultyMod;
                base.Health += (int)((float)(difficultyMod * difficultyMod) * 2f);
                base.ExperienceGained += difficultyMod;
                if (difficultyMod >= 5)
                {
                    Tags.Add("golem_difficulty_min_5");
                }
                if (difficultyMod >= 10)
                {
                    Tags.Add("golem_difficulty_min_10");
                }
                Sprite.currentFrame = 16;
                Sprite.loop = false;
                Sprite.UpdateSourceRect();
            }
        }
    }
}
