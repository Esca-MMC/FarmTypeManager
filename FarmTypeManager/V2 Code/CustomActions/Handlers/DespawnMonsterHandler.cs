using FarmTypeManager.TileQueries;
using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Monsters;
using System.Collections.Generic;
using System.Linq;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that despawns (removes) monsters.</summary>
    public class DespawnMonsterHandler : BasicHandlerBase<DespawnMonsterSettings>, ICustomActionHandler
    {
        protected override bool TryActionAtLocation(GameLocation location, DespawnMonsterSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int times, out string error)
        {
            List<MonsterMatchData> matchData = new(settings.MonsterMatchDataList ?? []);
            if (settings.MonsterMatchData != null)
                matchData.Insert(0, settings.MonsterMatchData);

            HashSet<Vector2> tilesToCheck; //a set of tiles matching the tile condition; null if no condition was used
            if (settings.TileCondition != null)
            {
                tilesToCheck = [];
                TileCondition tileCondition = new(location, settings.TileCondition); //NOTE: it'd be slower to add extra conditions for this type
                foreach (Vector2 tile in tileCondition.GetTiles())
                    tilesToCheck.Add(tile);
            }
            else
                tilesToCheck = null;

            List<int> indices = new(Enumerable.Range(0, location.characters.Count));
            Collections.RandomizeList(indices);

            List<int> indicesToRemove = [];

            foreach (int index in indices)
            {
                if (times <= 0) //if enough instances matched already, stop looking for more
                    break;

                Monster monster = location.characters[index] as Monster;
                if (monster == null)
                    continue;

                if (matchData.Count > 0 && matchData.Any(data => !data.Match(monster))) //if any data does NOT match this instance
                    continue;

                if (tilesToCheck != null && !tilesToCheck.Contains(monster.Tile)) //if this instance does NOT match the tile condition (TODO: consider checking all tiles within the monster's bounding box, if performance isn't bad)
                    continue;

                indicesToRemove.Add(index); //this instance matches, so mark it for removal
                times--;
            }

            indicesToRemove.Sort();
            for (int x = indicesToRemove.Count - 1; x >= 0; x--) //for each index to remove, looping backward to allow removal
            {
                int index = indicesToRemove[x];

                if (Properties.Monitor.IsVerbose)
                {
                    if (location.characters[index] is Monster monster && monster != null)
                    {
                        monster.modData.TryGetValue(Properties.ModDataKeys.SpawnId, out string spawnId);
                        Properties.Monitor.VerboseLog($"{nameof(DespawnMonsterHandler)}: Removing monster. Location: \"{location.NameOrUniqueName}\". Tile: {monster.Tile.X},{monster.Tile.Y}. Name: \"{monster.Name ?? "[null]"}\". Spawn ID: \"{spawnId ?? "[null]"}\".");
                    }
                }

                location.characters.RemoveAt(index);
            }

            error = null;
            return true;
        }
    }
}
