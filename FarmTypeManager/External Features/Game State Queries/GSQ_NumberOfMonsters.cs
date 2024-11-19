using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Monsters;

namespace FarmTypeManager.ExternalFeatures.GameStateQueries
{
    /// <summary>A game state query that checks how many monsters are in a named location.</summary>
    public class GSQ_NumberOfMonsters
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

            GameStateQuery.Register(prefix + "NUMBER_OF_MONSTERS", GSQ_NumberOfMonsters_Impl);

            Enabled = true;
        }

        /// <summary>Checks whether the number of <see cref="Monster"/> NPCs at a location matches the given numbers.</summary>
        /// <param name="query">The game state query split by space, including the query key.</param>
        /// <param name="context">The game state query context.</param>
        /// <returns>Returns true if the numbers of monsters at the location is within the minimum and optional maximum (inclusively).</returns>
        /// <remarks>
        /// Format: Esca.FarmTypeManager_NUMBER_OF_MONSTERS &lt;location&gt; &lt;min&gt; [max]
        /// <br/><br/>
        /// The location argument supports the special values used by base game GSQs, e.g. "Here" and "Target".
        /// </remarks>
        private static bool GSQ_NumberOfMonsters_Impl(string[] query, GameStateQueryContext context)
        {
            GameLocation location = context.Location; //get a location from the GSQ's context, if any

            //parse args; if any are missing or invalid, log an error and return false

            if (!GameStateQuery.Helpers.TryGetLocationArg(query, 1, ref location, out var error) || !ArgUtility.TryGetInt(query, 2, out int min, out error, "int minimum") || !ArgUtility.TryGetOptionalInt(query, 3, out int max, out error, int.MaxValue, "int maximum"))
            {
                return GameStateQuery.Helpers.ErrorResult(query, error); //log the resulting error (via the game itself, not the mod's logger) and return false
            }

            int monsterCount;
            if (location?.IsActiveLocation() == true) //if this location is active, i.e. the local player has accurate info about NPCs there
            {
                monsterCount = 0;
                foreach (var npc in location.characters)
                {
                    if (npc is Monster)
                        monsterCount++;
                }
            }
            else //if the location is NOT active
                monsterCount = -1; //special value for locations that currently aren't loaded/synced

            if (monsterCount >= min && monsterCount <= max)
                return true;
            else
                return false;
        }
    }
}