using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Netcode;

using Microsoft.Xna.Framework.Graphics;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A terrain feature representing an <see cref="StardewValley.Item"/> placed on the ground, similar to <see cref="StardewValley.Object"/>.</summary>
        /// <remarks>This class is not currently designed to be saved by the game's native processes. All instances should be removed from the game before saving (i.e. end of day).</remarks>
        public class PlacedItem : TerrainFeature
        {
            [XmlElement("Item")]
            public readonly NetRef<Item> item = new NetRef<Item>(null);
            /// <summary>The item represented by this class.</summary>
            public Item Item
            {
                get
                {
                    return item.Value;
                }

                set
                {
                    item.Value = value;
                }
            }

            public PlacedItem()
                : base(false) //needsTick = false, because this class doesn't use tick updates
            {
                initNetFields();
            }

            /// <summary>Create a new placed item.</summary>
            /// <param name="tileLocation">The tile location of the placed item.</param>
            /// <param name="item">The item contained by this object.</param>
            public PlacedItem(Vector2 tileLocation, Item item)
                : this() //call this class's default constructor
            {
                currentTileLocation = tileLocation;
                this.item.Value = item;
            }

            protected virtual void initNetFields()
            {
                NetFields.AddFields(item); //include this class's custom field
            }

            public override void draw(SpriteBatch spriteBatch, Vector2 tileLocation)
            {
                if (Item != null)
                {
                    Vector2 screenPosition = Game1.GlobalToLocal(tileLocation * 64);
                    Item.drawInMenu(spriteBatch, screenPosition, 1f, 1f, 0.01f, StackDrawType.Hide);
                }
                else if (currentLocation != null) //if this 
                {
                    currentLocation.terrainFeatures.Remove(tileLocation); //remove it from the game
                }
            }

            public override bool isPassable(Character c = null)
            {
                if (Constants.TargetPlatform == GamePlatform.Android) //if this is SDV Android
                    return true; //always treat placed items as passable
                else
                    return base.isPassable(c);
            }

            public override void doCollisionAction(Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location)
            {
                if (Constants.TargetPlatform == GamePlatform.Android) //if this is running on Android
                {
                    if (who is Farmer farmer && farmer.IsLocalPlayer) //if the colliding character is the local player
                    {
                        performUseAction(tileLocation, location); //attempt to pick up the item
                    }
                }
            }

            public override bool performUseAction(Vector2 tileLocation, GameLocation location)
            {
                if (!Game1.player.canMove || this.isTemporarilyInvisible || !Game1.player.couldInventoryAcceptThisItem(Item)) //if this isn't the local player OR they can't currently pick this up
                    return false; //this placed item was not used

                //add the contained item to the player's inventory and remove this placed item

                if (Game1.player.addItemToInventoryBool(Item, true)) //add this item to the player's inventory; if successful,
                {
                    location.localSound("pickUpItem");
                    DelayedAction.playSoundAfterDelay("coin", 300, location, -1);
                    Game1.player.animateOnce(279 + Game1.player.FacingDirection); //do the player's "pick up object" animation

                    if (Item.GetType() != typeof(StardewValley.Object) || (Item as StardewValley.Object).bigCraftable) //if this item is anything other than a basic StardewValley.Object
                    {
                        //prevent displaying the item during the "pick up object" animation
                        for (int x = 0; x < Game1.player.FarmerSprite.CurrentAnimation.Count; x++) //modify each frame of the player's "pick up object" animation
                        {
                            Game1.player.FarmerSprite.CurrentAnimation[x] = new FarmerSprite.AnimationFrame(
                                Game1.player.FarmerSprite.CurrentAnimation[x].frame,
                                Game1.player.FarmerSprite.CurrentAnimation[x].milliseconds,
                                Game1.player.FarmerSprite.CurrentAnimation[x].secondaryArm,
                                Game1.player.FarmerSprite.CurrentAnimation[x].flip,
                                null, //null the delegate that normally calls Farmer.showItemIntake (which always uses "springobjects" images)
                                false);
                        }
                    }

                    Item = null; //clear this placed item's reference to the item
                    currentLocation.terrainFeatures.Remove(tileLocation); //remove this placed item from the game
                }  

                return true; //this placed item was used
            }
        }
    }
}
