using FarmTypeManager.Utilities;
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

        /// <summary>The behavior to use when selecting locations.</summary>
        SelectionMode LocationListMode { get; set; }
    }
}
