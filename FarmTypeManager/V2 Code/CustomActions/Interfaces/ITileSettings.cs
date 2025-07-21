using StardewValley;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings that specify tiles within a <see cref="GameLocation"/>.</summary>
    public interface ITileSettings
    {
        /// <summary>A tile query. Used to retrieve a set of tiles with the specified conditions.</summary>
        string TileCondition { get; set; }
    }
}
