using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Monsters;

namespace FarmTypeManager.ExternalFeatures.GameStateQueries
{
    /// <summary>A game state query that checks whether a location currently exists.</summary>
    public class GSQ_LocationIsActive
    {
        /// <summary>True if this class's behavior is currently enabled.</summary>
		public static bool Enabled { get; private set; } = false;

        /// <summary>Initializes this class and enables its features.</summary>
        /// <param name="helper">The helper instance to use during initialization, e.g. to register events.</param>
        public static void Enable(IModHelper helper)
        {
            if (Enabled)
                return;

            string prefix = helper.ModRegistry.ModID + "_";

            GameStateQuery.Register(prefix + "LOCATION_IS_ACTIVE", GSQ_LocationIsActive_Impl);

            Enabled = true;
        }

        /// <summary>Checks whether the given name matches an existing, active location (i.e. the local player has up-to-date data for the location).</summary>
        /// <param name="query">The game state query split by space, including the query key.</param>
        /// <param name="context">The game state query context.</param>
        /// <returns>Returns true if a location in "Utility.ForEachLocation" has a matching name (NameOrUniqueName) and is actively synchronized for the local player.</returns>
        /// <remarks>
        /// Format: Esca.FarmTypeManager_LOCATION_IS_ACTIVE &lt;location&gt;
        /// <br/><br/>
        /// The location argument does NOT support the special values used by base game GSQs, e.g. "Here" and "Target".
        /// </remarks>
        private static bool GSQ_LocationIsActive_Impl(string[] query, GameStateQueryContext context)
        {
            //parse args; if any are missing or invalid, log an error and return false

            if (!ArgUtility.TryGet(query, 1, out string locationName, out string error, false, "location name"))
            {
                return GameStateQuery.Helpers.ErrorResult(query, error); //log the resulting error (via the game itself, not the mod's logger) and return false
            }

            bool matches = false; //true if a location exists with this name

            //loop through each location and see if the name matches
            Utility.ForEachLocation((location) =>
            {
                if (string.Equals(location?.NameOrUniqueName, locationName, System.StringComparison.OrdinalIgnoreCase)) //if the location's name matches (case-insensitive)
                {
                    matches = (location?.IsActiveLocation() ?? false);
                    return false; //stop looping
                }
                else
                    return true; //keep looping
            },
            true, true);

            return matches;
        }
    }
}