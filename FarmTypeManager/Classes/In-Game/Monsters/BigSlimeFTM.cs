using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Xml.Serialization;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A subclass of Stardew's BigSlime class, adjusted for use by this mod.</summary>
        public class BigSlimeFTM : BigSlime
        {
            [XmlElement("FTM_mineLevelOfDeathSpawns")]
            public readonly NetInt mineLevelOfDeathSpawns = new NetInt(0);

            /// <summary>A number that affects the type and/or stats of any monsters spawned by this monster's death.</summary>
            [XmlIgnore]
            public int MineLevelOfDeathSpawns
            {
                get
                {
                    return mineLevelOfDeathSpawns.Value;
                }

                set
                {
                    mineLevelOfDeathSpawns.Value = value;
                }
            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            public BigSlimeFTM()
                : base()
            {

            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            public BigSlimeFTM(Vector2 position)
                : base(position, 0)
            {
                MineLevelOfDeathSpawns = 0;
            }

            /// <summary>Creates an instance of Stardew's BigSlime class, but with adjustments made for this mod.</summary>
            /// <param name="position">The x,y coordinates of this monster's location.</param>
            /// <param name="mineLevel">A number that affects the type and/or stats of this monster. This normally represents which floor of the mines the monster spawned on (121+ for skull cavern).</param>
            public BigSlimeFTM(Vector2 position, int mineLevel)
                : base(position, mineLevel)
            {
                MineLevelOfDeathSpawns = mineLevel;
            }

            /// <summary>This method adds the CustomDamage setting to to the monster's list of net fields for multiplayer functionality.</summary>
            protected override void initNetFields()
            {
                base.initNetFields();
                this.NetFields.AddField(mineLevelOfDeathSpawns);
            }

            //this override fixes the following BigSlime behavioral bugs:
            //* spawned small slimes using "current mine level" even outside the mines
            //* spawned small slimes breeding in non-temporary locations
            public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
            {
                int actualDamage = Math.Max(1, damage - resilience.Value);
                if (Game1.random.NextDouble() < missChance.Value - missChance.Value * addedPrecision)
                {
                    actualDamage = -1;
                }
                else
                {
                    Slipperiness = 3;
                    Health -= actualDamage;
                    setTrajectory(xTrajectory, yTrajectory);
                    currentLocation.playSound("hitEnemy");
                    IsWalkingTowardPlayer = true;
                    if (Health <= 0)
                    {
                        deathAnimation();
                        if (Game1.gameMode == 3 && Game1.random.NextDouble() < 0.75)
                        {
                            int toCreate = Game1.random.Next(2, 5);
                            for (int i = 0; i < toCreate; i++)
                            {
                                GreenSlime slime = new(Position, MineLevelOfDeathSpawns) //use MineLevelOfDeathSpawns instead of checking the game state
                                {
                                    IsEphemeral = true,
                                    readyToMate = int.MaxValue //disable slime mating
                                }; 

                                currentLocation.characters.Add(slime);
                                slime.setTrajectory(xTrajectory / 8 + Game1.random.Next(-2, 3), yTrajectory / 8 + Game1.random.Next(-2, 3));
                                slime.willDestroyObjectsUnderfoot = false;
                                slime.moveTowardPlayer(4);
                                slime.Scale = 0.75f + Game1.random.Next(-5, 10) / 100f;
                                slime.currentLocation = currentLocation;
                            }
                        }
                    }
                }
                return actualDamage;
            }
        }
    }
}
