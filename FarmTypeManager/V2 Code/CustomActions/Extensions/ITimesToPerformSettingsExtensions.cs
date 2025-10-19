using StardewValley;
using System;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ITimesToPerformSettings"/> interface.</summary>
    public static class ITimesToPerformSettingsExtensions
    {
        /// <summary>Gets a random number within the range specified by settings in <see cref="ITimesToPerformSettings"/>.</summary>
        /// <typeparam name="T">A type that implements <see cref="ITimesToPerformSettings"/>.</typeparam>
        /// <param name="location">The location to use in game state query context, if any.</param>
        /// <param name="player">The player to use in game state query context, if any.</param>
        /// <param name="targetItem">The target item to use in game state query context, if any.</param>
        /// <param name="inputItem">The input item to use in game state query context, if any.</param>
        /// <param name="random">The random number generator to use in game state query context, if any. If not provided, this method will use <see cref="FTMUtility.Random"/>.</param>
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
    }
}
