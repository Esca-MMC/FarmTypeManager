using FarmTypeManager.Monsters;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using static FarmTypeManager.ModEntry;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Contains this mod's built-in monster type handlers, e.g. for types in the base game.</summary>
    public static class NativeMonsterTypeHandlers
    {
        /// <summary>Provides this mod's built-in monster type handlers, e.g. for types in the base game.</summary>
        public static Dictionary<string, MonsterTypeHandler> Get()
        {
            return new(StringComparer.OrdinalIgnoreCase)
            {
                //NOTE: Monster types are sorted by shared class or sub-class (e.g. all "Bat" class types are together) and then alphabetically by their keys.

                {
                    "Bat",
                    new(
                        (tile) => new BatFTM(tile, 0),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "FrostBat",
                    new(
                        (tile) => new BatFTM(tile, 40),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "LavaBat",
                    new(
                        (tile) => new BatFTM(tile, 80),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "IridiumBat",
                    new(
                        (tile) => new BatFTM(tile, 171),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "CursedDoll",
                    new(
                        (tile) => new BatFTM(tile, -666),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "HauntedSkull",
                    new(
                        (tile) => new BatFTM(tile, 77377),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "MagmaSprite",
                    new(
                        (tile) => new BatFTM(tile, -555),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "MagmaSparker",
                    new(
                        (tile) => new BatFTM(tile, -556),
                        null,
                        null,
                        Vector2.One
                    )
                },

                //TODO: refactor/replace big slimes' class to use the 2.0+ monster system for their small slime spawns (DayEnding removal is not yet implemented in that system, as of this writing)

                {
                    "BigGreenSlime",
                    new(
                        (tile) => new BigSlimeFTM(tile, 0),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText) && monster is BigSlimeFTM slime)
                            {
                                if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                    slime.c.Value = color;
                                else
                                    FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: BigGreenSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is BigSlimeFTM slime)
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.c.Value.R} {slime.c.Value.G} {slime.c.Value.B} {slime.c.Value.A}"; //save current color to mod data in "R G B A" format
                        },
                        new Vector2(2, 2)
                    )
                },
                {
                    "BigBlueSlime",
                    new(
                        (tile) => new BigSlimeFTM(tile, 40),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText) && monster is BigSlimeFTM slime)
                            {
                                if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                    slime.c.Value = color;
                                else
                                    FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: BigBlueSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is BigSlimeFTM slime)
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.c.Value.R} {slime.c.Value.G} {slime.c.Value.B} {slime.c.Value.A}"; //save current color to mod data in "R G B A" format
                        },
                        new Vector2(2, 2)
                    )
                },
                {
                    "BigRedSlime",
                    new(
                        (tile) => new BigSlimeFTM(tile, 80),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText) && monster is BigSlimeFTM slime)
                            {
                                if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                    slime.c.Value = color;
                                else
                                    FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: BigRedSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is BigSlimeFTM slime)
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.c.Value.R} {slime.c.Value.G} {slime.c.Value.B} {slime.c.Value.A}"; //save current color to mod data in "R G B A" format
                        },
                        new Vector2(2, 2)
                    )
                },
                {
                    "BigPurpleSlime",
                    new(
                        (tile) => new BigSlimeFTM(tile, 121),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText) && monster is BigSlimeFTM slime)
                            {
                                if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                    slime.c.Value = color;
                                else
                                    FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: BigPurpleSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is BigSlimeFTM slime)
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.c.Value.R} {slime.c.Value.G} {slime.c.Value.B} {slime.c.Value.A}"; //save current color to mod data in "R G B A" format
                        },
                        new Vector2(2, 2)
                    )
                },



                {
                    "BlueSquid",
                    new(
                        (tile) => new BlueSquid(tile),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "Bug",
                    new(
                        (tile) => new Bug(tile, 0),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "ArmoredBug",
                    new(
                        (tile) => new Bug(tile, 121),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "Duggy",
                    new(
                        (tile) => new DuggyFTM(tile),
                        (monster) =>
                        {
                            if (monster is DuggyFTM duggy)
                                duggy.DamageToFarmer = duggy.CustomDamage; //copy damage into the custom field used by this class
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },
                {
                    "MagmaDuggy",
                    new(
                        (tile) => new DuggyFTM(tile, true),
                        (monster) =>
                        {
                            if (monster is DuggyFTM duggy)
                                duggy.DamageToFarmer = duggy.CustomDamage; //copy damage into the custom field used by this class
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },



                {
                    "DustSprite",
                    new(
                        (tile) => new DustSpirit(tile),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "Ghost",
                    new(
                        (tile) => new GhostFTM(tile),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "CarbonGhost",
                    new(
                        (tile) => new GhostFTM(tile, "Carbon Ghost"),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "PutridGhost",
                    new(
                        (tile) => new GhostFTM(tile, "Putrid Ghost"),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "GreenSlime",
                    new(
                        (tile) => new GreenSlime(tile, 0) { readyToMate = int.MaxValue }, //disable mating
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        slime.color.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: GreenSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                                {
                                    //apply known gender values to slimes' "cute" field, which is used in breeding code
                                    if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = true;
                                    else if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = false;
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText))
                                {
                                    if (int.TryParse(segmentsText, out int segments))
                                    {
                                        segments = Math.Max(0, segments); //treat negative values as 0
                                        slime.stackedSlimes.Value = segments;
                                    }
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: GreenSlime. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.color.Value.R} {slime.color.Value.G} {slime.color.Value.B} {slime.color.Value.A}"; //overwrite with current color in "R G B A" format
                                slime.modData[FTMUtility.ModDataKeys.Gender] = slime.cute.Value ? "M" : "F"; //overwrite with "cute" field: true = "M", false = "F"
                                slime.modData[FTMUtility.ModDataKeys.Segments] = slime.stackedSlimes.Value.ToString();
                            }
                        },
                        Vector2.One
                    )
                },
                {
                    "BlueSlime",
                    new(
                        (tile) => new GreenSlime(tile, 40) { readyToMate = int.MaxValue },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        slime.color.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: BlueSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                                {
                                    //apply known gender values to slimes' "cute" field, which is used in breeding code
                                    if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = true;
                                    else if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = false;
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText))
                                {
                                    if (int.TryParse(segmentsText, out int segments))
                                    {
                                        segments = Math.Max(0, segments); //treat negative values as 0
                                        slime.stackedSlimes.Value = segments;
                                    }
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: BlueSlime. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.color.Value.R} {slime.color.Value.G} {slime.color.Value.B} {slime.color.Value.A}"; //overwrite with current color in "R G B A" format
                                slime.modData[FTMUtility.ModDataKeys.Gender] = slime.cute.Value ? "M" : "F"; //overwrite with "cute" field: true = "M", false = "F"
                                slime.modData[FTMUtility.ModDataKeys.Segments] = slime.stackedSlimes.Value.ToString();
                            }
                        },
                        Vector2.One
                    )
                },
                {
                    "RedSlime",
                    new(
                        (tile) => new GreenSlime(tile, 80) { readyToMate = int.MaxValue },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        slime.color.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: RedSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                                {
                                    //apply known gender values to slimes' "cute" field, which is used in breeding code
                                    if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = true;
                                    else if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = false;
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText))
                                {
                                    if (int.TryParse(segmentsText, out int segments))
                                    {
                                        segments = Math.Max(0, segments); //treat negative values as 0
                                        slime.stackedSlimes.Value = segments;
                                    }
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: RedSlime. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.color.Value.R} {slime.color.Value.G} {slime.color.Value.B} {slime.color.Value.A}"; //overwrite with current color in "R G B A" format
                                slime.modData[FTMUtility.ModDataKeys.Gender] = slime.cute.Value ? "M" : "F"; //overwrite with "cute" field: true = "M", false = "F"
                                slime.modData[FTMUtility.ModDataKeys.Segments] = slime.stackedSlimes.Value.ToString();
                            }
                        },
                        Vector2.One
                    )
                },
                {
                    "PurpleSlime",
                    new(
                        (tile) => new GreenSlime(tile, 121) { readyToMate = int.MaxValue },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        slime.color.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: PurpleSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                                {
                                    //apply known gender values to slimes' "cute" field, which is used in breeding code
                                    if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = true;
                                    else if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = false;
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText))
                                {
                                    if (int.TryParse(segmentsText, out int segments))
                                    {
                                        segments = Math.Max(0, segments); //treat negative values as 0
                                        slime.stackedSlimes.Value = segments;
                                    }
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: PurpleSlime. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.color.Value.R} {slime.color.Value.G} {slime.color.Value.B} {slime.color.Value.A}"; //overwrite with current color in "R G B A" format
                                slime.modData[FTMUtility.ModDataKeys.Gender] = slime.cute.Value ? "M" : "F"; //overwrite with "cute" field: true = "M", false = "F"
                                slime.modData[FTMUtility.ModDataKeys.Segments] = slime.stackedSlimes.Value.ToString();
                            }
                        },
                        Vector2.One
                    )
                },
                {
                    "TigerSlime",
                    new(
                        (tile) =>
                        {
                            GreenSlime slime = new(tile, 0) { readyToMate = int.MaxValue };
                            slime.makeTigerSlime();
                            return slime;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        slime.color.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: TigerSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                                {
                                    //apply known gender values to slimes' "cute" field, which is used in breeding code
                                    if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = true;
                                    else if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = false;
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText))
                                {
                                    if (int.TryParse(segmentsText, out int segments))
                                    {
                                        segments = Math.Max(0, segments); //treat negative values as 0
                                        slime.stackedSlimes.Value = segments;
                                    }
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: TigerSlime. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.color.Value.R} {slime.color.Value.G} {slime.color.Value.B} {slime.color.Value.A}"; //overwrite with current color in "R G B A" format
                                slime.modData[FTMUtility.ModDataKeys.Gender] = slime.cute.Value ? "M" : "F"; //overwrite with "cute" field: true = "M", false = "F"
                                slime.modData[FTMUtility.ModDataKeys.Segments] = slime.stackedSlimes.Value.ToString();
                            }
                        },
                        Vector2.One
                    )
                },
                {
                    "PrismaticSlime",
                    new(
                        (tile) =>
                        {
                            GreenSlime slime = new(tile, 0) { readyToMate = int.MaxValue };
                            slime.makePrismatic();
                            return slime;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        slime.color.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: PrismaticSlime. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Gender, out string gender))
                                {
                                    //apply known gender values to slimes' "cute" field, which is used in breeding code
                                    if (gender.StartsWith("M", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = true;
                                    else if (gender.StartsWith("F", StringComparison.OrdinalIgnoreCase))
                                        slime.cute.Value = false;
                                }

                                if (slime.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText))
                                {
                                    if (int.TryParse(segmentsText, out int segments))
                                    {
                                        segments = Math.Max(0, segments); //treat negative values as 0
                                        slime.stackedSlimes.Value = segments;
                                    }
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: PrismaticSlime. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is GreenSlime slime)
                            {
                                slime.modData[FTMUtility.ModDataKeys.Color] = $"{slime.color.Value.R} {slime.color.Value.G} {slime.color.Value.B} {slime.color.Value.A}"; //overwrite with current color in "R G B A" format
                                slime.modData[FTMUtility.ModDataKeys.Gender] = slime.cute.Value ? "M" : "F"; //overwrite with "cute" field: true = "M", false = "F"
                                slime.modData[FTMUtility.ModDataKeys.Segments] = slime.stackedSlimes.Value.ToString();
                            }
                        },
                        Vector2.One
                    )
                },

                

                //TODO: refactor/replace grubs' class to use the 2.0+ monster system for their fly spawns (DayEnding removal is not yet implemented in that system, as of this writing)

                {
                    "Grub",
                    new(
                        (tile) => new GrubFTM(tile, false),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "MutantGrub",
                    new(
                        (tile) => new GrubFTM(tile, true),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "Fly",
                    new(
                        (tile) => new FlyFTM(tile, false),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "MutantFly",
                    new(
                        (tile) => new FlyFTM(tile, true),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "MetalHead",
                    new(
                        (tile) => new MetalHead(tile, 0),
                        (monster) =>
                        {
                            if (monster is MetalHead metal)
                            {
                                if (metal.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        metal.c.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: MetalHead. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is MetalHead metal && metal.c.Value != Color.White) //if this isn't using the default color
                                metal.modData[FTMUtility.ModDataKeys.Color] = $"{metal.c.Value.R} {metal.c.Value.G} {metal.c.Value.B} {metal.c.Value.A}"; //overwrite with current color in "R G B A" format
                        },
                        Vector2.One
                    )
                },
                {
                    "HotHead", //NOTE: grouped with MetalHead due to similarity and being a sub-class
                    new(
                        (tile) => new HotHead(tile),
                        (monster) =>
                        {
                            if (monster is HotHead hot)
                            {
                                if (hot.modData.TryGetValue(FTMUtility.ModDataKeys.Color, out string colorText))
                                {
                                    if (FTMUtility.TryParseColor(colorText, out Color color, out string colorError))
                                        hot.c.Value = color;
                                    else
                                        FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom color. Using default color instead. Monster type: HotHead. Parsing error message: \n{colorError}", LogLevel.Debug);
                                }
                            }
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is MetalHead hot && hot.c.Value != Color.White) //if this isn't using the default color
                                hot.modData[FTMUtility.ModDataKeys.Color] = $"{hot.c.Value.R} {hot.c.Value.G} {hot.c.Value.B} {hot.c.Value.A}"; //overwrite with current color in "R G B A" format
                        },
                        Vector2.One
                    )
                },



                {
                    "LavaLurk",
                    new(
                        (tile) => new LavaLurkFTM(tile),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string rangedText) && rangedText.StartsWith("t", StringComparison.OrdinalIgnoreCase) && monster is LavaLurkFTM lava)
                                lava.RangedAttacks = false;
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },



                {
                    "Mummy",
                    new(
                        (tile) => new MummyFTM(tile),
                        null, //TODO: test saving & loading revive timer and any necessary related fields, so that quick-save doesn't auto-revive temporily dead mummies
                        null,
                        Vector2.One
                    )
                },



                {
                    "PepperRex",
                    new(
                        (tile) => new DinoMonster(tile),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string disableRanged) && disableRanged.StartsWith("t", StringComparison.OrdinalIgnoreCase) && monster is DinoMonster dino)
                            {
                                //disable ranged attacks by altering attack values
                                dino.timeUntilNextAttack = int.MaxValue;
                                dino.nextFireTime = int.MaxValue;
                            }
                            return monster;
                        },
                        null,
                        new Vector2(2, 2)
                    )
                },



                {
                    "RockCrab",
                    new(
                        (tile) => new RockCrab(tile),
                        null, //TODO: test saving type-specific fields, e.g. to preserve shell damage
                        null,
                        Vector2.One
                    )
                },
                {
                    "LavaCrab",
                    new(
                        (tile) => new RockCrab(tile, "Lava Crab"),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "IridiumCrab",
                    new(
                        (tile) => new RockCrab(tile, "Iridium Crab"),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "FalseMagmaCap",
                    new(
                        (tile) =>
                        {
                            var monster = new RockCrab(tile, "False Magma Cap");
                            monster.HideShadow = true; //set by preference; makes them look more like the objects they mimic
                            return monster;
                        },
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "StickBug",
                    new(
                        (tile) =>
                        {
                            var monster = new RockCrab(tile);
                            monster.makeStickBug();
                            return monster;
                        },
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "TruffleCrab",
                    new(
                        (tile) =>
                        {
                            var monster = new RockCrab(tile, "Truffle Crab");
                            monster.HideShadow = true; //set by preference; makes them look more like the objects they mimic
                            return monster;
                        },
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "StoneGolem",
                    new(
                        (tile) => new RockGolemFTM(tile),
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "WildernessGolem",
                    new(
                        (tile) => new RockGolemFTM(tile, Math.Min(8, Game1.player.CombatLevel)), //limit the combat skill check to 8, which circumvents the chance to create an iridium golem
                        null,
                        null,
                        Vector2.One
                    )
                },
                {
                    "IridiumGolem",
                    new(
                        (tile) => new RockGolemFTM(tile, Game1.player.CombatLevel, true),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "Serpent",
                    new(
                        (tile) => new SerpentFTM(tile),
                        null,
                        null,
                        new Vector2(2, 2)
                    )
                },
                {
                    "RoyalSerpent",
                    new(
                        (tile) => new SerpentFTM(tile, "Royal Serpent"),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.Segments, out string segmentsText) && monster is SerpentFTM serpent)
                            {
                                if (int.TryParse(segmentsText, out int segments))
                                {
                                    segments = Math.Max(2, segments); //royal serpents must have at least 2
                                    serpent.segmentCount.Value = segments;
                                }
                                else
                                    FTMUtility.Monitor.LogOnce($"Couldn't parse a monster's custom \"segments\" number. Using default segments instead. Monster type: RoyalSerpent. Unparsed text: \n{segmentsText}", LogLevel.Debug);
                            }

                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is SerpentFTM serpent)
                                serpent.modData[FTMUtility.ModDataKeys.Segments] = serpent.segmentCount.Value.ToString();
                        },
                        new Vector2(2, 2)
                    )
                },



                {
                    "ShadowBrute",
                    new(
                        (tile) => new ShadowBrute(tile),
                        null,
                        null,
                        Vector2.One
                    )
                },



                {
                    "ShadowShaman",
                    new(
                        (tile) => new ShadowShaman(tile),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string disableRanged) && disableRanged.StartsWith("t", StringComparison.OrdinalIgnoreCase) && monster is ShadowShaman shaman)
                                shaman.coolDown = int.MaxValue; //disable ranged abilities with an unreachable cooldown
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },



                {
                    "ShadowSniper",
                    new(
                        (tile) => new Shooter(tile),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string disableRanged) && disableRanged.StartsWith("t", StringComparison.OrdinalIgnoreCase) && monster is Shooter sniper)
                                sniper.nextShot = float.MaxValue; //disable ranged attack with an unreachable cooldown
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },



                {
                    "Skeleton",
                    new(
                        (tile) => new SkeletonFTM(tile),
                        (monster) =>
                        {
                            if (monster is SkeletonFTM skeleton)
                            {
                                if (skeleton.focusedOnFarmers) //if customization set this to true
                                {
                                    //adjust related fields to cause the intended behavior
                                    skeleton.spottedPlayer = true;
                                    skeleton.IsWalkingTowardPlayer = true;
                                }

                                if (skeleton.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string disableRanged) && disableRanged.StartsWith("t", StringComparison.OrdinalIgnoreCase))
                                    skeleton.rangedAttacks.Value = false;
                            }
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },
                {
                    "SkeletonMage",
                    new(
                        (tile) => new SkeletonFTM(tile, true),
                        (monster) =>
                        {
                            if (monster is SkeletonFTM skeleton)
                            {
                                if (skeleton.focusedOnFarmers) //if customization set this to true
                                {
                                    //adjust related fields to cause the intended behavior
                                    skeleton.spottedPlayer = true;
                                    skeleton.IsWalkingTowardPlayer = true;
                                }

                                if (skeleton.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string disableRanged) && disableRanged.StartsWith("t", StringComparison.OrdinalIgnoreCase))
                                    skeleton.rangedAttacks.Value = false;
                            }
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },



                {
                    "Spider",
                    new(
                        (tile) => new Leaper(tile),
                        null, //TODO: test saving type-specific fields, e.g. leap state
                        null,
                        Vector2.One
                    )
                },



                {
                    "Spiker",
                    new(
                        (tile) => new Spiker(tile, 0), //use default facing direction (unknown here) and correct it below
                        (monster) =>
                        {
                            if (monster is Spiker spiker)
                                spiker.targetDirection = spiker.FacingDirection; //copy potentially customized field into the type-specific field
                            return monster;
                        },
                        (monster) =>
                        {
                            if (monster is Spiker spiker)
                                spiker.FacingDirection = spiker.targetDirection; //copy type-specific field into the field that will be serialized
                        },
                        Vector2.One
                    )
                },



                {
                    "SquidKid",
                    new(
                        (tile) => new SquidKidFTM(tile),
                        (monster) =>
                        {
                            if (monster.modData.TryGetValue(FTMUtility.ModDataKeys.DisableRangedAttacks, out string disableRanged) && disableRanged.StartsWith("t", StringComparison.OrdinalIgnoreCase) && monster is SquidKid squid)
                                squid.lastFireball = float.MaxValue; //disable ranged attack with an unreachable cooldown
                            return monster;
                        },
                        null,
                        Vector2.One
                    )
                },
            };
        }
    }
}
