using StardewValley;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify <see cref="GameLocation"/>s.</summary>
    public interface ILocationSettings
    {
        /// <summary>The internal name of a single <see cref="GameLocation"/>.</summary>
        string Location { get; set; }
        /// <summary>A list of internal names for <see cref="GameLocation"/>s.</summary>
        List<string> LocationList { get; set; }
        /// <summary>The behavior to use when selecting locations from <see cref="LocationList"/>.</summary>
        LocationListModes LocationListMode { get; set; }

        /// <summary>The available values of <see cref="ILocationSettings.LocationListMode"/>.</summary>
        public enum LocationListModes
        {
            /// <summary>All locations from the list should be used. For example, if an action spawns objects, it should spawn a full set of objects at each listed location.</summary>
            All,
            /// <summary>Locations from the list should be used in random order.</summary>
            Random
        }
    }
}
