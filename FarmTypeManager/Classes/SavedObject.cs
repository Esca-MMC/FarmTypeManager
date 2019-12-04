using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewValley.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A class containing all necessary information about an in-game object.</summary>
        private class SavedObject
        {
            public string MapName { get; set; }
            public Vector2 Tile { get; set; }
            public ObjectType Type { get; set; }
            public int? ID { get; set; }
            public string Name { get; set; }
            public int? DaysUntilExpire { get; set; }
            public MonsterType MonType { get; set; }
            public StardewTime SpawnTime { get; set; } = 600; //default to 6:00AM for backward compatibility

            /// <summary>A point representing this object's size in tiles.</summary>
            [JsonIgnore]
            public Point Size
            {
                get
                {
                    switch (Type)
                    {
                        case ObjectType.Forage:
                        case ObjectType.Ore:
                            return new Point(1, 1);
                        case ObjectType.LargeObject:
                            if (ID == 190 || ID == 254 || ID == 276) //if this seems to be a giant crop
                                return new Point(3, 3);
                            else //if this seems to be a resource clump
                                return new Point(2, 2);
                        case ObjectType.Monster:
                            if (!MonsterSizeCache.ContainsKey(MonType.MonsterName)) //if this monster type's size has not been cached yet
                            {
                                if (MonType.MonsterName.Contains(".")) //if this is an external monster class
                                {
                                    Type externalType = Utility.GetTypeFromName(MonType.MonsterName, typeof(Monster)); //find a monster subclass with a matching name
                                    Monster monster = (Monster)Activator.CreateInstance(externalType, Vector2.Zero); //create a monster with the Vector2 constructor (the tile should be irrelevant)
                                    int width = Convert.ToInt32(Math.Ceiling(((double)monster.Sprite.SpriteWidth) / 16)); //get the monster's sprite width in tiles, rounded up
                                    if (width <= 0)
                                        width = 1; //enforce minimum 1x1 size
                                    MonsterSizeCache.Add(MonType.MonsterName, new Point(width, width)); //use the width for both dimensions (to avoid problems with "tall" 1x1 monster sprites)
                                }
                                else if //if this is a known type of 2x2 monster
                                (
                                    MonType.MonsterName.StartsWith("big", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("serpent", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("rex", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("pepper", StringComparison.OrdinalIgnoreCase) ||
                                    MonType.MonsterName.StartsWith("dino", StringComparison.OrdinalIgnoreCase)
                                )
                                    MonsterSizeCache.Add(MonType.MonsterName, new Point(2, 2));
                                else //if this is a known type of 1x1 monster
                                    MonsterSizeCache.Add(MonType.MonsterName, new Point(1, 1));
                            }

                            return MonsterSizeCache[MonType.MonsterName];
                        default:
                            return new Point(1, 1); //use default size for unknown enum values
                    }
                }
            }

            private static Dictionary<string, Point> MonsterSizeCache = new Dictionary<string, Point>();

            /// <param name="mapName">The name of the in-game location where this object exists.</param>
            /// <param name="tile">A tile indicating where this object exists.</param>
            /// <param name="id">The object's ID, often called parentSheetIndex.</param>
            /// <param name="type">The enumerated spawn type of the object, e.g. Forage.</param>
            /// <param name="name">The object's name. Used informally by spawners that do not rely on ID.</param>
            /// <param name="daysUntilExpire">The remaining number of days before this object should be removed from the game.</param>
            /// <param name="monsterType">The monster type spawn data used to respawn a monster; null if this isn't a monster.</param>
            /// <param name="spawnTime">The specific in-game time at which this object will spawn. Uses Stardew's internal time format, i.e. multiples of 10 from 600 to 2600.</param>
            public SavedObject(string mapName, Vector2 tile, ObjectType type, int? id, string name, int? daysUntilExpire, MonsterType monsterType = null, StardewTime spawnTime = default(StardewTime))
            {
                MapName = mapName;
                Tile = tile;
                Type = type;
                ID = id;
                Name = name;
                DaysUntilExpire = daysUntilExpire;
                MonType = monsterType;
                SpawnTime = spawnTime;
            }

            /// <summary>Enumerated list of object types managed by Farm Type Manager. Used to process saved object information.</summary>
            public enum ObjectType { Forage, LargeObject, Ore, Monster }
        }
    }
}