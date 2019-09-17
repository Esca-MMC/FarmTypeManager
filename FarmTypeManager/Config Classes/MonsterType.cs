using System;
using System.Collections.Generic;
using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>A container for a monster's name and a set of optional customization settings.</summary>
        private class MonsterType
        {
            public string MonsterName { get; set; } = ""; //a string representing a specific monster type

            private Dictionary<string, object> settings = null; //a dictionary of optional setting names and values (of various types)
            public Dictionary<string, object> Settings
            {
                get
                {
                    return settings;
                }
                set
                {
                    if (value != null && value.Comparer != StringComparer.OrdinalIgnoreCase) //if the provided dictionary exists & isn't using a case-insensitive comparer
                    {
                        //create and use a copy with a case-insensitive comparer
                        settings = new Dictionary<string, object>(value, StringComparer.OrdinalIgnoreCase);
                    }
                    else
                    {
                        settings = value;
                    }
                }
            }

            public MonsterType(string monsterName, Dictionary<string, object> settings)
            {
                MonsterName = monsterName;
                Settings = settings;
            }
        }
    }
}