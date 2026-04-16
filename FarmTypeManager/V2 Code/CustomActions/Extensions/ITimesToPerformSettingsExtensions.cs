using FarmTypeManager.Utilities;
using StardewValley;
using StardewValley.Delegates;
using System;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ITimesToPerformSettings"/> interface.</summary>
    public static class ITimesToPerformSettingsExtensions
    {
        /// <summary>Gets a random number within the range specified by settings in <see cref="ITimesToPerformSettings"/>.</summary>
        /// <typeparam name="T">A type that implements <see cref="ITimesToPerformSettings"/>.</typeparam>
        /// <param name="location">The location to use when checking conditions.</param>
        /// <param name="player">The player to use when checking conditions.</param>
        /// <param name="targetItem">The target item to use when checking conditions.</param>
        /// <param name="inputItem">The input item to use when checking conditions.</param>
        /// <param name="random">The random number generator to use when checking conditions. If not provided, <see cref="FTMUtility.Random"/> will be used. Note that <see cref="FTMUtility.Random"/> will always be used in other logic; this argument only affects conditions.</param>
        /// <returns>A random number within the range specified by settings in <see cref="ITimesToPerformSettings"/>.</returns>
        public static int GetRandomTimes<T>(this T settings, GameLocation location, Farmer player, Item targetItem, Item inputItem, Random random) where T : ITimesToPerformSettings
        {
            int startingNum = FTMUtility.Random.Next(settings.MinTimes, settings.MaxTimes + 1); //get a random number between min and max (inclusive)

            if (settings.TimesModifiers?.Count > 0)
            {
                float modifiedNum = Utility.ApplyQuantityModifiers(startingNum, settings.TimesModifiers, settings.TimesModifierMode, location, player, targetItem, inputItem, random ?? FTMUtility.Random); //apply modifiers with any provided GSQ context
                return (int)Math.Round(modifiedNum); //round to the nearest integer
            }
            else
                return startingNum;
        }

        /// <inheritdoc cref="GetRandomTimes{T}(T, GameLocation, Farmer, Item, Item, Random)"/>
        /// <param name="queryContext">The context to use when checking conditions, if any.</param>
        public static int GetRandomTimes<T>(this T settings, GameStateQueryContext queryContext) where T : ITimesToPerformSettings
            => GetRandomTimes(settings, queryContext.Location, queryContext.Player, queryContext.TargetItem, queryContext.InputItem, queryContext.Random);
    }
}
