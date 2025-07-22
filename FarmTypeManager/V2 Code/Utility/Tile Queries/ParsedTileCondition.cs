using System;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A set of parsed information about a single condition inside a tile query.</summary>
    /// <param name="args">The tile condition's key and any arguments provided.</param>
    /// <param name="handler">The handler associated with this tile condition.</param>
    public class ParsedTileCondition(string[] args, ITileConditionHandler handler) : IComparable<ParsedTileCondition>
    {
        /**************/
        /* Properties */
        /**************/

        /// <summary>The tile condition's key and any arguments provided.</summary>
        public string[] Args { get; init; } = args;
        /// <summary>The handler that implements this tile condition.</summary>
        public ITileConditionHandler Handler { get; init; } = handler;

        /***************/
        /* IComparable */
        /***************/

        public int CompareTo(ParsedTileCondition other)
        {
            int thisPriority;
            int otherPriority;

            if (Args?.Length > 0 && Args[0] != null && Handler != null)
                Handler.ConditionPriorities.TryGetValue(Args[0], out thisPriority);
            else
                thisPriority = ITileConditionHandler.EvalPriority_Normal;

            if (other.Args?.Length > 0 && other.Args[0] != null && other.Handler != null)
                other.Handler.ConditionPriorities.TryGetValue(other.Args[0], out otherPriority);
            else
                otherPriority = ITileConditionHandler.EvalPriority_Normal;

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
