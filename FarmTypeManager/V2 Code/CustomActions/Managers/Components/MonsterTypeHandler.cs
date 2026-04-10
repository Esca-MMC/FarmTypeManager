using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System;

namespace FarmTypeManager.CustomActions
{
    /******************/
    /* Nested classes */
    /******************/

    /// <summary>Handles creation, customization, and data tracking for a specific monster type.</summary>
    /// <param name="create">A method called to create an instance of this monster type. Input argument is the pixel position where monster will be placed.</param>
    /// <param name="customize">A method called after FTM applies generic customizations. Applies any type-specific customizations to a monster, e.g. unique property values from mod data. May be null.</param>
    /// <param name="updateData">A method called after FTM updates generic customization/serialization data (e.g. before saving). Stores or updates any type-specific mod data for a monster, e.g. unique property values. May be null.</param>
    /// <param name="tileSize">The size of this monster type in tiles.</param>
    public class MonsterTypeHandler(Func<Vector2, Monster> create, Func<Monster, Monster> customize, Action<Monster> updateData, Vector2 tileSize)
    {
        /// <summary>A method called to create an instance of this monster type. Input argument is the pixel position where monster will be placed.</summary>
        public Func<Vector2, Monster> Create = create;

        /// <summary>A method called after FTM applies generic customizations. Applies any type-specific customizations to a monster, e.g. unique property values from mod data. May be null.</summary>
        public Func<Monster, Monster> Customize = customize;

        // <summary>A method called after FTM updates generic customization/serialization data (e.g. before saving). Stores or updates any type-specific mod data for a monster, e.g. unique property values. May be null.</summary>
        public Action<Monster> UpdateData = updateData;

        /// <summary>The size of this monster type in tiles.</summary>
        public Vector2 TileSize = tileSize;
    }
}
