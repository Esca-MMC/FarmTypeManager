using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI;

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
            public int ID { get; set; }
            public string Name { get; set; }
            public int? DaysUntilExpire { get; set; }
            public MonsterType MonType { get; set; }

            /// <param name="mapName">The name of the in-game location where this object exists.</param>
            /// <param name="tile">A tile indicating where this object exists.</param>
            /// <param name="id">The object's ID, often called parentSheetIndex.</param>
            /// <param name="type">The general type of the object.</param>
            /// <param name="name">The object's name. Used informally by spawners that do not rely on ID.</param>
            /// <param name="daysUntilExpire">The remaining number of days before this object should be removed from the game.</param>
            /// <param name="monsterType">The monster type spawn data used to respawn a monster; null if this isn't a monster.</param>
            public SavedObject(string mapName, Vector2 tile, ObjectType type, int id, string name, int? daysUntilExpire, MonsterType monsterType = null)
            {
                MapName = mapName;
                Tile = tile;
                Type = type;
                ID = id;
                Name = name;
                DaysUntilExpire = daysUntilExpire;
                MonType = monsterType;
            }

            /// <summary>Enumerated list of object types managed by Farm Type Manager. Used to process saved object information.</summary>
            public enum ObjectType { Forage, LargeObject, Ore, Monster }
        }
    }
}