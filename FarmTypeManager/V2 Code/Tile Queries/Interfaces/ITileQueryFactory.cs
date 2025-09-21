using StardewValley;
using System;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A factory used to register and generate instances of <see cref="ITileQuery"/> with specific types.</summary>
    public interface ITileQueryFactory
    {
        /// <summary>Creates a new tile query instance to handle the given arguments.</summary>
        /// <param name="location">The in-game location to check.</param>
        /// <param name="queryArgs">The text of the query to handle, split by spaces with quote awareness. The first argument is the query key.</param>
        /// <remarks>
        /// <para>Some query keys start with the '!' character, which indicates it should use "NOT" logic and invert its results. These are treated as distinct, separate query keys. To support them, a factory should be registered with each version of the key.</para>
        /// <para>If any errors occur while creating a tile query, throw an appropriate <see cref="Exception"/>. Null return values will also be handled safely, but will result in less descriptive log messages.</para>
        /// </remarks>
        /// <returns>A new tile query instance that can handle the given query, if possible.</returns>
        ITileQuery CreateTileQuery(GameLocation location, string[] queryArgs);
    }
}