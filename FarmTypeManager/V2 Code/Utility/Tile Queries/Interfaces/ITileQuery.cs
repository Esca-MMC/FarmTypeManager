using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A tile query handler. Performs a specific boolean check on tiles in a <see cref="GameLocation"/>.</summary>
    public interface ITileQuery
    {
        /*************/
        /* Constants */
        /*************/

        /// <summary>A recommended priority value for very fast methods, e.g. methods that always return a predefined or cached value.</summary>
        const int Priority_VeryHigh = 2000;
        /// <summary>A recommended priority value for fast methods, e.g. math-based comparisons that don't check tile properties.</summary>
        const int Priority_High = 1000;
        /// <summary>A recommended priority value for average methods, e.g. tile property comparisons.</summary>
        const int Priority_Normal = 0;
        /// <summary>A recommended priority value for by slow methods, e.g. complex tile property comparisons, or methods that check additional queries or tiles.</summary>
        const int Priority_Low = -1000;
        /// <summary>A recommended priority value for very slow methods, e.g. very complex comparisons that involve many methods, queries, and/or tiles.</summary>
        const int Priority_VeryLow = -2000;
        /// <summary>The priority value for optional methods that should never be used.</summary>
        const int Priority_NotImplemented = int.MinValue;

        /**************/
        /* Properties */
        /**************/

        /// <summary>The priority level of this query's <see cref="CheckTile"/> method.</summary>
        /// <remarks>
        /// <para>Queries with higher priority checks will be used before other queries in the same condition. This value can be based on sub-queries or arguments, but should not change after initialization.</para>
        /// <para>Faster, more efficient methods should have higher priority than slower, less efficient methods. See the constant values in <see cref="ITileQuery"/> and the built-in implementations for reference.</para>
        /// </remarks>
        int CheckTilePriority { get; }

        /// <summary>The priority level of this query's <see cref="GetStartingTiles"/> method.</summary>
        /// <remarks>
        /// <para>If this value is higher than every other query in a tile condition, <see cref="GetStartingTiles"/> will be used to generate the starting tile list.</para>
        /// <para>This value can be based on sub-queries or arguments, but should not change after initialization.</para>
        /// <para>If this value is <see cref="int.MinValue"/>, this query's <see cref="GetStartingTiles"/> method will not be used.</para>
        /// </remarks>
        int StartingTilesPriority { get; }

        /***********/
        /* Methods */
        /***********/

        /// <summary>Checks whether this tile is considered valid by this query.</summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if this tile is considered valid by this query.</returns>
        bool CheckTile(Vector2 tile);

        /// <summary>Creates a list of all tiles considered valid by this query.</summary>
        /// <remarks>
        /// <para>If <see cref="StartingTilesPriority"/> is <see cref="Priority_NotImplemented"/>, this method will not be used.</para>
        /// <para>This should only be implemented if this query can create a full list quickly, e.g. if it can validate all tiles through simple math without per-tile checks. See the built-in implementations for reference.</para>
        /// </remarks>
        /// <returns>All tiles considered valid by this query.</returns>
        List<Vector2> GetStartingTiles();
    }
}