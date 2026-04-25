using FarmTypeManager.Utilities;
using StardewValley;
using StardewValley.Delegates;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ILocationSettings"/> interface.</summary>
    public static class ILocationSettingsExtensions
    {
        /// <summary>Get all specified locations that are active for the current local player.</summary>
        /// <param name="contextLocationName">The unique name of <see cref="GameStateQueryContext.Location"/>. Null if query context is unavailable.</param>
        /// <returns>A list of all specificied locations that are active for the current local player.</returns>
        /// <remarks>This uses any location names in <see cref="ILocationSettings"/>. It does not account for <see cref="LocationListMode"/>.</remarks>
        public static List<GameLocation> GetActiveLocations<T>(this T settings, string contextLocationName = null) where T : ILocationSettings
        {
            List<string> nameList = [];

            if (settings.Location != null)
                nameList.AddRange(Locations.GetLocationNames(settings.Location, contextLocationName));

            if (settings.LocationList != null)
                foreach (string name in settings.LocationList)
                    nameList.AddRange(Locations.GetLocationNames(name, contextLocationName));

            List<GameLocation> locationList = [];

            foreach (string name in nameList)
                if (Locations.GetLocationIfActive(name) is GameLocation location)
                    locationList.Add(location);

            return locationList;
        }

        /// <summary>Yields a set of active locations from these settings, each paired with the number of times to use that location (e.g. to perform an action there).</summary>
        /// <param name="timesToSelect">The number of times to perform an action, or to perform it for each location, depending on selection mode.</param>
        /// <param name="contextLocationName">The unique name of <see cref="GameStateQueryContext.Location"/>. Null if query context is unavailable.</param>
        /// <returns>A yielded series of active locations to use, each paired with the number of times to use that location. Any unused locations will be excluded.</returns>
        public static IEnumerable<(GameLocation, int)> GetActiveLocationsAndTimes<T>(this T settings, int timesToSelect, string contextLocationName = null) where T : ILocationSettings
        {
            if (timesToSelect < 1)
                yield break;

            List<GameLocation> activeLocations = settings.GetActiveLocations(contextLocationName);
            if (activeLocations.Count < 1)
                yield break;

            List<int> timesToUseThisIndex = new(new int[activeLocations.Count]); //create a parallel list of "times to use" for each location list index (and initialize values to 0)

            foreach (int index in Collections.SelectElementsByMode(Enumerable.Range(0, activeLocations.Count).ToList(), settings.LocationListMode, timesToSelect)) //select indices to use
                timesToUseThisIndex[index]++; //increment each selected index's "times to use"

            for (int x = 0; x < activeLocations.Count; x++) //for each location index
                if (timesToUseThisIndex[x] > 0) //if it should be used at all
                    yield return (activeLocations[x], timesToUseThisIndex[x]); //yield the location and the number of times to use it
        }
    }
}
