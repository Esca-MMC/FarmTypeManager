using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.TileQueries
{
    /// <summary>A handler for the "HAS_CHARACTER" tile query. Allows tiles that contain a character (farmer, NPC, animal, etc).</summary>
    public class HasCharacterTileQuery : ITileQuery
    {
        /***************/
        /* Constructor */
        /***************/

        /// <param name="location">The in-game location to check.</param>
        public HasCharacterTileQuery(GameLocation location)
        {
            Location = location;
        }

        /**************/
        /* Properties */
        /**************/

        /// <summary>The in-game location to check.</summary>
        private GameLocation Location { get; }

        /**************/
        /* ITileQuery */
        /**************/

        public int CheckTilePriority => ITileQuery.Priority_Low;
        public int StartingTilesPriority => ITileQuery.Priority_NotImplemented;
        public bool CheckTile(Vector2 tile) => TileHasCharacter(tile);
        public List<Vector2> GetStartingTiles() => throw new NotImplementedException();

        /*****************/
        /* Other methods */
        /*****************/

        /// <summary>Checks whether any character at this location exists on, or overlaps with, the given tile.</summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns>True if any character exists on the tile or overlaps with it.</returns>
        private bool TileHasCharacter(Vector2 tile)
        {
            int x = (int)((tile.X + 0.5f) * 64f);
            int y = (int)((tile.Y + 0.5f) * 64f);
            Point position = new(x, y);

            foreach (Farmer farmer in Location.farmers)
                if (farmer.Tile == tile)
                    return true;

            foreach (Character character in Location.characters)
                if (character.GetBoundingBox().Contains(position))
                    return true;

            foreach (FarmAnimal animal in Location.animals.Values)
                if (animal.GetBoundingBox().Contains(position))
                    return true;

            return false;
        }
    }
}