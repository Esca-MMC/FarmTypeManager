using System;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A set of parsed information about a single condition inside a tile query.</summary>
    /// <param name="args">The tile condition's key and any arguments provided.</param>
    /// <param name="handler">The handler associated with this tile condition.</param>
    public class ParsedTileCondition(string[] args, ITileConditionHandler handler) : IComparable<ParsedTileCondition>
    {
        /*************/
        /* Constants */
        /*************/

        /// <summary>The <see cref="Priority"/> value used by "high priority" built-in conditions.</summary>
        public const int EvalPriority_High = 1000;
        /// <summary>The <see cref="Priority"/> value used by "normal priority" built-in conditions.</summary>
        public const int EvalPriority_Normal = 0;
        /// <summary>The <see cref="Priority"/> value used by "low priority" built-in conditions.</summary>
        public const int EvalPriority_Low = -1000;

        /**************/
        /* Properties */
        /**************/

        /// <summary>The tile condition's key and any arguments provided.</summary>
        public string[] Args { get; init; } = args;
        /// <summary>The evaluation priority level for this tile condition. Higher priority causes this condition to be evaluated earlier than others in the same query.</summary>
        /// <remarks><para>In general, simpler conditions should have higher priority than complex conditions, allowing them to be evaluated less often.</para><para>For example, a condition that repeatedly checks map properties might be slower than others; a lower priority should improve its performance.</para></remarks>
        public ITileConditionHandler Handler { get; init; } = handler;

        /***************/
        /* IComparable */
        /***************/

        public int CompareTo(ParsedTileCondition other)
        {
            int thisPriority;
            int otherPriority;

            if (this.Args?.Length > 0 && this.Args[0] != null && this.Handler != null)
                this.Handler.ConditionPriorities.TryGetValue(this.Args[0], out thisPriority);
            else
                thisPriority = EvalPriority_Normal;

            if (other.Args?.Length > 0 && other.Args[0] != null && other.Handler != null)
                other.Handler.ConditionPriorities.TryGetValue(other.Args[0], out otherPriority);
            else
                otherPriority = EvalPriority_Normal;

            return thisPriority.CompareTo(otherPriority);
        }

        public static bool operator <(ParsedTileCondition left, ParsedTileCondition right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(ParsedTileCondition left, ParsedTileCondition right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(ParsedTileCondition left, ParsedTileCondition right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >=(ParsedTileCondition left, ParsedTileCondition right)
        {
            return left.CompareTo(right) >= 0;
        }
    }
}
