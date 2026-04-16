using FarmTypeManager.Serialization;
using FarmTypeManager.Utilities;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The handler for a custom action that spawns <see cref="Monster"/> instances.</summary>
    public class SpawnMonsterHandler : SpawnHandlerBase<SpawnMonsterSettings, MonsterData>
    {
        /*********************************/
        /* SpawnHandlerBase - Properties */
        /*********************************/

        protected override string LogTextForAnInstance => "a monster";

        protected override string LogTextForInstances => "monsters";

        /******************************/
        /* SpawnHandlerBase - Methods */
        /******************************/

        protected override IEnumerable<MonsterData> CreateInstances(SpawnMonsterSettings settings, GameStateQueryContext queryContext, TriggerActionContext triggerContext, int numberOfTimes)
        {
            if (numberOfTimes <= 0)
                yield break;

            List<SpawnMonsterData> list = [];
            if (settings.Monster != null)
                list.Add(settings.Monster);
            if (settings.MonsterList != null)
                list.AddRange(settings.MonsterList);

            if (list?.Count < 1)
                yield break;

            int? spawnLimit;
            if (FTMUtility.MConfig?.MonsterLimitPerLocation.HasValue == true) //if the player has set a monster limit
            {
                int monstersAtLocation = 0; //count any monsters in the query context location
                foreach (var character in queryContext.Location?.characters ?? [])
                    if (character is Monster)
                        monstersAtLocation++;

                spawnLimit = FTMUtility.MConfig.MonsterLimitPerLocation.Value - monstersAtLocation;
                if (spawnLimit <= 0)
                {
                    if (FTMUtility.Monitor.IsVerbose)
                        FTMUtility.Monitor.VerboseLog($"Skipping \"SpawnMonster\" custom action due to the player config's monster limit. Location \"{queryContext.Location?.NameOrUniqueName}\" has {monstersAtLocation} monsters; the limit is {FTMUtility.MConfig.MonsterLimitPerLocation.Value}.");
                    yield break;
                }
            }
            else
                spawnLimit = null;

            foreach (SpawnMonsterData entry in list.GetWeightedConditionalElements(settings.MonsterListMode, numberOfTimes, queryContext)) //get monster spawn data to use
            {
                if (entry.ChanceToSkip > 0 && FTMUtility.Random.NextDouble() < entry.ChanceToSkip)
                    continue;

                foreach (MonsterData monsterData in entry.CreateMonsterData(queryContext)) //create data for specific monsters from this spawn data
                {
                    yield return monsterData;

                    if (spawnLimit != null && --spawnLimit <= 0) //if spawn limit exists, reduce it by 1; if no more monsters should be yielded, stop here (NOTE: these monsters aren't guaranteed to be placed, but this is effective enough for an optional setting)
                        yield break;
                }
            }
        }

        protected override string GetId(MonsterData instance) => instance?.SpawnId ?? base.GetId(instance);

        protected override Vector2 GetSize(MonsterData instance) => MonsterManager.GetSize(instance.SpawnId) ?? Vector2.One;

        protected override bool TrySpawn(GameLocation location, Vector2 tile, MonsterData instance, out string placementError)
        {
            instance.Position = tile * 64f; //set position, which isn't known at data creation time

            Monster monster;
            try
            {
                monster = MonsterManager.Create(instance, location);

                if (monster == null)
                {
                    if (instance?.SpawnId == null)
                        placementError = "Failed to create a monster because its ID was null.";
                    else
                        placementError = $"Failed to create a monster because its ID is not recognized.";

                    return false;
                }

                MonsterSerializer.Add(monster, instance, location.NameOrUniqueName);
            }
            catch (Exception ex)
            {
                placementError = $"Failed to create a monster due to an error. Full error message: {ex}";
                return false;
            }

            monster.currentLocation = location; //set location, which isn't known at creation time (including for base game monsters)
            location.addCharacter(monster);
            placementError = null;
            return true;
        }
    }
}
