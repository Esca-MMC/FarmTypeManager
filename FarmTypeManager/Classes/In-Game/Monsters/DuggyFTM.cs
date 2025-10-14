using Microsoft.Xna.Framework;
using Netcode;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System.Xml.Serialization;
using xTile.Layers;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's Duggy class, adjusted for use by this mod.</summary>
    public class DuggyFTM : Duggy, ICustomDamage
    {
        [XmlElement("FTM_customDamage")]
        public readonly NetInt customDamage = new(8); //default set to mimic hardcoded values

        /// <summary>A customizable value for DamageToFarmer, used to preserve it during temporary damage changes.</summary>
        [XmlIgnore]
        public int CustomDamage
        {
            get
            {
                return customDamage.Value;
            }

            set
            {
                customDamage.Value = value;
            }
        }

        /// <summary>Creates an instance of Stardew's Duggy class, but with adjustments made for this mod.</summary>
        public DuggyFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's Duggy class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public DuggyFTM(Vector2 position)
            : base(position)
        {

        }

        /// <summary>Creates an instance of Stardew's Duggy class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <param name="magmaDuggy">True if this should be a Magma Duggy. NOTE: Currently redundant; this constructor always produces a Magma Duggy.</param>
        public DuggyFTM(Vector2 position, bool magmaDuggy)
            : base(position, magmaDuggy)
        {

        }

        /// <summary>This method adds the CustomDamage setting to to the monster's list of net fields for multiplayer functionality.</summary>
        protected override void initNetFields()
        {
            base.initNetFields();
            this.NetFields.AddField(customDamage);
        }

        //This override fixes the following Duggy behavioral bugs:
        // * error that prevented multiplayer farmhands from loading the game while these monsters exist (null location/map/layer data)
        public override void update(GameTime time, GameLocation location)
        {
            if (invincibleCountdown > 0)
            {
                glowingColor = Color.Cyan;
                invincibleCountdown -= time.ElapsedGameTime.Milliseconds;
                if (invincibleCountdown <= 0)
                {
                    stopGlowing();
                }
            }
            if (location.farmers.Any())
            {
                behaviorAtGameTick(time);
                Layer backLayer = location?.map?.RequireLayer("Back"); //null check the location and map before trying to load the layer
                if (backLayer != null) //if the layer exists, allow the normal self-removal check
                {
                    if (base.Position.X < 0f || base.Position.X > (float)(backLayer.LayerWidth * 64) || base.Position.Y < 0f || base.Position.Y > (float)(backLayer.LayerHeight * 64))
                    {
                        location.characters.Remove(this);
                    }
                }
                updateGlow();
                if (stunTime.Value > 0)
                {
                    stunTime.Value -= (int)time.ElapsedGameTime.TotalMilliseconds;
                }
            }
        }

        //This override fixes the following Duggy behavioral bugs:
        // * permanently editing tiles' TileIndex (attempting to display the "empty hole" sprite)
        // * failing to un-burrow in most game locations
        // * using hard-coded damage values, making it non-customizable
        public override void behaviorAtGameTick(GameTime time)
        {
            Monster_behaviorAtGameTick(time); //call this manual implementation rather than the "base" method, due to the way nested subclasses work
            isEmoting = false;
            Sprite.loop = false;
            if (stunTime.Value > 0)
            {
                return;
            }
            Rectangle r = GetBoundingBox();
            if (Sprite.currentFrame < 4)
            {
                r.Inflate(128, 128);
                if (!base.IsInvisible || r.Contains(base.Player.StandingPixel))
                {
                    if (base.IsInvisible)
                    {
                        if (currentLocation?.map != null) //if the player has access to the current location's map (a necessary check for farmhands in some locations)
                        {
                            //only check for the NPCBarrier flag, ignoring the base Duggy's other movement restrictions (e.g. for "Diggable" or index 0)
                            if (currentLocation.map.RequireLayer("Back").Tiles[Player.TilePoint.X, Player.TilePoint.Y].Properties.ContainsKey("NPCBarrier"))
                                return;
                        }
                        base.Position = new Vector2(base.Player.Position.X, base.Player.Position.Y + (float)base.Player.Sprite.SpriteHeight - (float)Sprite.SpriteHeight);
                        base.currentLocation.localSound("Duggy");
                        base.Position = base.Player.Tile * 64f;
                    }
                    base.IsInvisible = false;
                    Sprite.interval = 100f;
                    Sprite.AnimateDown(time);
                }
            }
            if (Sprite.currentFrame >= 4 && Sprite.currentFrame < 8)
            {
                r.Inflate(-128, -128);
                base.currentLocation.isCollidingPosition(r, Game1.viewport, isFarmer: false, 8, glider: false, this);
                Sprite.AnimateRight(time);
                Sprite.interval = 220f;
                base.DamageToFarmer = CustomDamage; //use customizable damage instead of hardcoded values
            }
            if (Sprite.currentFrame >= 8)
            {
                Sprite.AnimateUp(time);
            }
            if (Sprite.currentFrame >= 10)
            {
                base.IsInvisible = true;
                Sprite.currentFrame = 0;
                //skip the base Duggy's tile alterations
                base.DamageToFarmer = 0;
            }
        }

        /// <summary>Except where commented, this is a copy of "Monster.behaviorAtGameTick", used to implement this monster's "base.behaviorAtGameTick" call.</summary>
        private void Monster_behaviorAtGameTick(GameTime time)
        {
            if (timeBeforeAIMovementAgain > 0f)
            {
                timeBeforeAIMovementAgain -= time.ElapsedGameTime.Milliseconds;
            }
        }
    }
}
