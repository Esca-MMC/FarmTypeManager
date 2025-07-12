using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>Handles a set of tile conditions for the tile query system.</summary>
    public interface ITileConditionHandler
    {
        /*************/
        /* Constants */
        /*************/

        /// <summary>The <see cref="EvalPriority"/> value used by "high priority" built-in conditions.</summary>
        const int EvalPriority_High = 1000;
        /// <summary>The <see cref="EvalPriority"/> value used by "normal priority" built-in conditions.</summary>
        const int EvalPriority_Normal = 0;
        /// <summary>The <see cref="EvalPriority"/> value used by "low priority" built-in conditions.</summary>
        const int EvalPriority_Low = -1000;

        /**************/
        /* Properties */
        /**************/

        /// <summary>A set of tile condition keys handled by this instance. Values are the evaluation priority level for a specific tile condition and arguments.</summary>
        /// <returns>The priority level for the provided condition and arguments.</returns>
        /// <remarks>
        /// <para>
        /// Higher priority causes a condition to be evaluated earlier than others in a query.
        /// Simpler conditions should have higher priority than complex conditions, allowing complex conditions to be evaluated less often.
        /// </para>
        /// <para>
        /// For example, conditions that only perform basic math on each tile should have high priority.
        /// Conditions that use non-lazy evaluation, and/or read a lot of map data to evaluate one tile, should have low priority.
        /// </para>
        /// </remarks>
        IDictionary<string, int> ConditionPriorities { get; }

        /***********/
        /* Methods */
        /***********/

        /// <summary>Returns any of the given tiles that match the arguments and context.</summary>
        /// <param name="args">The arguments for a single tile condition. The first element is the condition key.</param>
        /// <param name="location">The in-game location to check.</param>
        /// <param name="tiles">The set tiles to check.</param>
        /// <returns>Any of the given tiles that match the arguments and context.</returns>
        /// <remarks>The enumerables involved here are expected to be yield-based, allowing lazy evaluation. Do not iterate over the input enumerable more than once, and if possible, evaluate and yield one tile at a time.</remarks>
        IEnumerable<Vector2> FilterTiles(string[] args, GameLocation location, IEnumerable<Vector2> tiles);
    }
}