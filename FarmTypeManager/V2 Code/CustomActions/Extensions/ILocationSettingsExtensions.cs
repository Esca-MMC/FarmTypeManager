using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="ILocationSettings"/> interface.</summary>
    public static class ILocationSettingsExtensions
    {
        /// <summary>Get all specified locations that are active for the current local player.</summary>
        /// <returns>A list of all specificied locations that are active for the current local player.</returns>
        /// <remarks>This uses any location names in <see cref="ILocationSettings"/>. It does not account for <see cref="LocationListMode"/>.</remarks>
        public static List<GameLocation> GetActiveLocations<T>(this T settings) where T : ILocationSettings
        {
            List<string> nameList = [];

            if (settings.Location != null)
                nameList.AddRange(FTMUtility.GetAllLocationsFromName(settings.Location));

            if (settings.LocationList != null)
                foreach (string name in settings.LocationList)
                    nameList.AddRange(FTMUtility.GetAllLocationsFromName(name));

            List<GameLocation> locationList = [];

            foreach (string name in nameList)
                if (FTMUtility.GetLocationIfActive(name) is GameLocation location)
                    locationList.Add(location);

            return locationList;
        }
    }
}
