using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Provides utilities to spawn all supported <see cref="Monster"/> instances, and manages other tasks like serialization.</summary>
    public static class MonsterManager
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>A set of handlers for known monster type IDs.</summary>
        private static readonly Dictionary<string, MonsterTypeHandler> Handlers = NativeMonsterHandlers.Get();

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Creates a new monster instance with this data.</summary>
        /// <param name="data">A set of monster data to use.</param>
        /// <param name="location">The location to set in this monster's <see cref="Character.currentLocation"/> field. May be null.</param>
        /// <returns>A new monster instance based on the provided data. Null if the data is null or invalid, e.g. the monster type ID has no registered handler.</returns>
        public static Monster Create(MonsterData data, GameLocation location = null)
        {
            if (data == null || data.SpawnId == null || !Handlers.TryGetValue(data.SpawnId, out var handler)) //if data is null or invalid
                return null;

            Monster monster = handler.Create(data.Position ?? Vector2.Zero); //provide a default position if available

            if (location != null)
                monster.currentLocation = location;

            data.ApplyToMonster(monster); //apply all generic customization data
            handler.Customize?.Invoke(monster); //let handler apply type-specific customizations, if any
            return monster;
        }

        /// <summary>Get a monster type's size in tiles. Null if the ID is not registered.</summary>
        /// <param name="monsterId">The monster type's spawn ID. Case-insensitive.</param>
        /// <returns>The monster type's size in tiles, or null if the ID is not registered.</returns>
        public static Vector2? GetSize(string monsterId)
        {
            if (monsterId == null || !Handlers.TryGetValue(monsterId, out var handler))
                return null;

            return handler.TileSize;
        }

        /// <summary>Registers a handler for a specific monster type.</summary>
        /// <param name="monsterTypeId">The ID to associate with this monster type.</param>
        /// <param name="create">A method called to create an instance of this monster type. Input argument is the pixel position where monster will be placed.</param>
        /// <param name="customize">A method called after FTM applies generic customizations. Applies any type-specific customizations to a monster, e.g. unique property values from mod data. May be null.</param>
        /// <param name="updateData">A method called after FTM updates generic customization/serialization data (e.g. before saving). Stores or updates any type-specific mod data for a monster, e.g. unique property values. May be null.</param>
        /// <param name="tileSize">The size of this monster type in tiles.</param>
        public static void RegisterHandler(string monsterTypeId, Func<Vector2, Monster> create, Func<Monster, Monster> customize, Action<Monster> updateData, Vector2 tileSize)
            => Handlers[monsterTypeId] = new MonsterTypeHandler(create, customize, updateData, tileSize);

        /// <summary>Updates a set of data based on the monster instance's current state.</summary>
        /// <param name="monster">The monster instance associated with this data.</param>
        /// <param name="data">The data to update.</param>
        /// <returns>True if the data was successfully updated. False if the monster or data is null, or if the monster's type ID has no registered handler.</returns>
        public static bool UpdateData(Monster monster, MonsterData data)
        {
            if (monster == null || data == null || !Handlers.TryGetValue(data.SpawnId, out var handler))
                return false;

            handler.UpdateData?.Invoke(monster); //update specialized data if necessary for this handler
            data.UpdateFromMonster(monster);
            return true;
        }
    }
}
