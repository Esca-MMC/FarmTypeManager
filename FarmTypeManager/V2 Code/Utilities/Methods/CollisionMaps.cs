using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.Utilities
{
    /// <summary>Static methods used with collision maps, which are strings that represent 2D maps of tiles (e.g. "XX\nOO\nXX").</summary>
    public static class CollisionMaps
    {
        /// <summary>Gets a list of tile coordinates for each "impassable" tile in a collision map, offset relative to the "zero" tile (0,0).</summary>
        /// <param name="collisionMap">A string representing a collision map.</param>
        /// <returns>A list of tile coordinates for each "collision" tile in a collision map, offset relative to the "zero" tile (0,0).</returns>
        /// <remarks>
        /// <para>
        /// A collision map is a string that represents a map of "passable" and "impassable" tiles in a 2D area.
        /// These are partially based on the base game's collision maps, e.g. as used in this field: <see cref="StardewValley.GameData.Buildings.BuildingData.CollisionMap"/>
        /// </para>
        /// <para>
        /// Characters in collision maps are parsed as described below.
        /// If no "zero" tile (0,0) is marked on the map, the top left corner will be used.
        /// Whitespace between lines will be ignored.
        /// Any other unrecognized characters, including interior whitespace, will be treated as passable tiles (e.g. "_" will behave the same way as "O").
        /// <list type="bullet">
        ///     <item>
        ///         <term>X</term>
        ///         <description>An impassable tile, i.e. a tile with collision.</description>
        ///     </item>
        ///     <item>
        ///         <term>O</term>
        ///         <description>A passable tile, i.e. a tile without collision. Default behavior for any unrecognized characters.</description>
        ///     </item>
        ///     <item>
        ///         <term>\n</term>
        ///         <description>A line break symbol. Used to divide tiles onto separate lines; note that normal, in-editor line breaks will also do so. For example, "XXXOOOXXX" is a 1x9 horizontal line. "XXX\nOOO\nXXX" is a 3x3 square.</description>
        ///     </item>
        ///     <item>
        ///         <term>#</term>
        ///         <description>The "zero" tile of this map. Returned tile coordinates are relative to this tile; only one "zero" tile should exist in a map. This symbol makes the tile <b>impassable</b>, i.e. a tile with collision.</description>
        ///     </item>
        ///     <item>
        ///         <term>@</term>
        ///         <description>The "zero" tile of this map. Returned tile coordinates are relative to this tile; only one "zero" tile should exist in a map. This symbol makes the tile <b>passable</b>, i.e. a tile without collision.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        ///     <item>
        ///         <term>XXX</term>
        ///         <description>Returned tiles: 0,0 / 1,0 / 2,0</description>
        ///     </item>
        ///     <item>
        ///         <term>XXX \n OOO \n XXX</term>
        ///         <description>Returned tiles: 0,0 / 1,0 / 2,0 / 0,2 / 1,2 / 2,2</description>
        ///     </item>
        ///     <item>
        ///         <term>XXX \n O@O \n XXX</term>
        ///         <description>Returned tiles: -1,-1 / 0,-1 / 1,-1 / -1,1 / 0,1 / 1,1</description>
        ///     </item>
        ///     <item>
        ///         <term>XXX \n O#O \n XXX</term>
        ///         <description>Returned tiles: -1,-1 / 0,-1 / 1,-1 / 0,0 / -1,1 / 0,1 / 1,1</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        public static List<Vector2> Parse(string collisionMap)
        {
            if (string.IsNullOrWhiteSpace(collisionMap))
                return [];

            string[] lines = collisionMap.Split('\n', StringSplitOptions.TrimEntries);

            Vector2? zeroTile = null;
            for (int y = 0; y < lines.Length; y++) //for each line
                for (int x = 0; x < lines[y].Length; x++) //for each character in the line
                    if (lines[y][x] is '@' or '#')
                        if (zeroTile == null)
                            zeroTile = new(x, y);
                        else
                            throw new ArgumentException("Collision map contains more than one \"zero tile\" character ('@' or '#').");

            if (zeroTile == null)
                zeroTile = Vector2.Zero;

            List<Vector2> tiles = [];

            for (int y = 0; y < lines.Length; y++) //for each line
                for (int x = 0; x < lines[y].Length; x++) //for each character in the line
                    if (lines[y][x] is 'X' or '#')
                        tiles.Add(new Vector2(x - zeroTile.Value.X, y - zeroTile.Value.Y));

            return tiles;
        }
    }
}
