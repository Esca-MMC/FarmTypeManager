using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
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

        /// <summary>True after reflection-based monster type handlers have been generated.</summary>
        private static bool AddedReflectionHandlers = false;

        /// <summary>A set of handlers for known monster type IDs.</summary>
        private static readonly Dictionary<string, MonsterTypeHandler> Handlers = NativeMonsterTypeHandlers.Get();

        /******************/
        /* Public methods */
        /******************/

        /// <summary>Creates a new monster instance with this data.</summary>
        /// <param name="data">A set of monster data to use.</param>
        /// <param name="location">The location to set in this monster's <see cref="Character.currentLocation"/> field. May be null.</param>
        /// <returns>A new monster instance based on the provided data. Null if the data is null or invalid, e.g. the monster type ID has no registered handler.</returns>
        public static Monster Create(MonsterData data, GameLocation location = null)
        {
            if (data?.SpawnId == null) //if data is null or invalid
                return null;

            var handler = GetHandler(data.SpawnId);
            if (handler == null)
                return null;

            Monster monster = handler.Create(data.Position ?? Vector2.Zero); //provide a starting position if available

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
            if (monsterId == null)
                return null;

            var handler = GetHandler(monsterId);
            if (handler == null)
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
            if (monster == null || data == null)
                return false;

            GetHandler(data.SpawnId)?.UpdateData?.Invoke(monster); //update specialized data, if possible and necessary for this handler
            data.UpdateFromMonster(monster);
            return true;
        }

        /*******************/
        /* Private methods */
        /*******************/
        
        /// <summary>Gets the handler for a monster type.</summary>
        /// <param name="spawnId">The spawn ID registered with this handler.</param>
        /// <returns>The handler for the monster type. Null if the ID has no registered handler.</returns>
        private static MonsterTypeHandler GetHandler(string spawnId)
        {
            if (spawnId == null)
                return null;

            if (Handlers.TryGetValue(spawnId, out var handler))
                return handler; 

            if (!AddedReflectionHandlers) //if a handler was not found, and reflection handlers have not been generated yet
            {
                AddReflectionHandlers();
                if (Handlers.TryGetValue(spawnId, out handler)) //try again
                    return handler;
            }

            return null;
        }

        /// <summary>Creates reflection-based handlers for any compatible custom monster types, then adds them to <see cref="Handlers"/>.</summary>
        private static void AddReflectionHandlers()
        {
            if (AddedReflectionHandlers)
                return;
            AddedReflectionHandlers = true;

            foreach (Type type in Reflection.GetAllSubclassTypes(typeof(Monster)))
            {
                try
                {
                    if (type.FullName == null || Handlers.ContainsKey(type.FullName) || type.GetConstructor([typeof(Vector2)]) == null) //if this type is null, already handled, or doesn't have the necessary constructor
                        continue;

                    if (type.Assembly.GetName().Name is "Stardew Valley" or "StardewValley" or "FarmTypeManager") //if this type's assembly should be ignored
                        continue;

                    if (Activator.CreateInstance(type, Vector2.Zero) is Monster monster) //create a test instance of this type
                    {
                        //NOTE: This code estimates the tile area size needed to place this monster. It may be inaccurate.
                        //      In those cases (and whenever possible), the monster should use a custom handler instead, e.g. one registered via API by the providing mod.

                        int width = (int)Math.Ceiling(monster.Sprite.SpriteWidth / 16d); //get its sprite width in tiles, rounded up
                        if (width <= 0)
                            width = 1; //minimum 1x1 tile

                        Vector2 tileSize = new(width, width); //assume square tile size for placement

                        //register a handler that creates via reflection (with no special customization or field serialization)
                        Handlers.Add(type.FullName, new(
                            (tile) => (Monster)Activator.CreateInstance(type, tile),
                            null,
                            null,
                            tileSize
                        ));

                        if (Properties.Monitor.IsVerbose)
                            Properties.Monitor.VerboseLog($"Created a reflection-based handler for monster type: \"{type.FullName}\".");
                    }
                }
                catch (Exception ex)
                {
                    if (Properties.Monitor.IsVerbose)
                        Properties.Monitor.VerboseLog($"{nameof(NativeMonsterTypeHandlers)} skipped a reflected monster type due to an error. Type: {type.FullName ?? "(null)"}. Error: \n{ex}");
                }
            }
        }
    }
}
