using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

// Public API interface from Item Extensions' source code: https://github.com/misty-spring/StardewMods/blob/main/ItemExtensions/Api.cs

namespace ItemExtensions
{
    public interface IApi
    {
        /// <summary>
        /// Checks for a qualified id in modded clump data.
        /// </summary>
        /// <param name="qualifiedItemId">Qualified item ID.</param>
        /// <returns>Whether this id is a clump's.</returns>
        bool IsClump(string qualifiedItemId);

        /// <summary>
        /// Tries to spawn a clump.
        /// </summary>
        /// <param name="itemId">The clump ID.</param>
        /// <param name="position">Tile position.</param>
        /// <param name="location">Location to use.</param>
        /// <param name="error">Error string, if applicable.</param>
        /// <param name="avoidOverlap">Avoid overlapping with other clumps.</param>
        /// <returns>Whether spawning succeeded.</returns>
        bool TrySpawnClump(string itemId, Vector2 position, GameLocation location, out string error, bool avoidOverlap = false);
    }
}
