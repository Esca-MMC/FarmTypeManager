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
        /// <remarks>
        ///     <para>Case-insensitive. Recognized values:</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <term>All</term>
        ///             <description>Default. All locations from the list should be used at once. For example, an object-spawning action should spawn a copy of each object at every listed location.</description>
        ///         </item>
        ///         <item>
        ///             <term>Random</term>
        ///             <description>Locations should be selected in random order.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        string LocationListMode { get; set; }
    }
}
