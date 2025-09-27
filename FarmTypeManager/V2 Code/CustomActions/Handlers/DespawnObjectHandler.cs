using FarmTypeManager.TileQueries;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Delegates;
using System.Collections.Generic;
using Object = StardewValley.Object;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that despawns (removes) forage, craftables, or other basic placed <see cref="Object"/>s.</summary>
    public class DespawnObjectHandler : LocationHandlerBase<DespawnObjectSettings>, ICustomActionHandler
    {
        protected override bool TryActionAtLocation(GameLocation location, DespawnObjectSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int times, out string error)
        {
            List<ItemMatchData> matchData = new(settings.ItemMatchDataList ?? []);
            if (settings.ItemMatchData != null)
                matchData.Insert(0, settings.ItemMatchData);

            var tileCondition = new TileCondition(location, $"HAS_OBJECT, {settings.TileCondition}"); //create tile condition, limit it to tiles with objects
            var tiles = tileCondition.GetTiles(true).GetEnumerator();

            for (int x = 0; x < times; x++)
            {
                if (!tiles.MoveNext()) //if no more tiles exist, stop
                    break;

                Vector2 tile = tiles.Current;

                if (!location.Objects.TryGetValue(tile, out Object obj) || obj is null) //try to get the object at this tile; if none exists, skip it
                    continue;

                if (matchData.Count < 1) //if no match data was provided, then all items match
                {
                    location.Objects.Remove(tile);

                    if (FTMUtility.Monitor.IsVerbose)
                        FTMUtility.Monitor.VerboseLog($"{nameof(DespawnObjectHandler)}: Removing a placed object. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. Object ID: \"{obj.QualifiedItemId}\".");
                }
                else
                {
                    foreach (var data in matchData)
                    {
                        if (data.Match(obj, location, queryContext))
                        {
                            location.Objects.Remove(tile);

                            if (FTMUtility.Monitor.IsVerbose)
                                FTMUtility.Monitor.VerboseLog($"{nameof(DespawnObjectHandler)}: Removing a placed object. Location: \"{location.NameOrUniqueName}\". Tile: {tile.X},{tile.Y}. Object ID: \"{obj.QualifiedItemId}\".");
                            break;
                        }
                    }
                }
            }

            error = null;
            return true;
        }
    }
}
