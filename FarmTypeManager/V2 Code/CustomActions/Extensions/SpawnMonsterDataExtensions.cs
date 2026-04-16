using FarmTypeManager.Utilities;
using StardewValley.Delegates;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Extension methods for the <see cref="SpawnMonsterData"/> class.</summary>
    public static class SpawnMonsterDataExtensions
    {
        /*********************/
        /* Extension methods */
        /*********************/

        /// <summary>Creates specific data for new monster instances from this spawn data.</summary>
        /// <param name="queryContext">The game state query context to use when checking conditions.</param>
        /// <returns>A yielded set of specific data for new monster instances. May be empty.</returns>
        public static IEnumerable<MonsterData> CreateMonsterData(this SpawnMonsterData spawnData, GameStateQueryContext queryContext)
        {
            if (spawnData.MinTimes > spawnData.MaxTimes)
                throw new ArgumentOutOfRangeException(nameof(spawnData), $"Invalid monster spawn data: MinTimes ({spawnData.MinTimes}) is greater than MaxTimes ({spawnData.MaxTimes}).");

            int times = spawnData.GetRandomTimes(queryContext);

            List<string> monsterIDs = [];
            if (spawnData.Id != null)
                monsterIDs.Add(spawnData.Id);
            if (spawnData.IdList != null)
                monsterIDs.AddRange(spawnData.IdList);


            List<string> colorStrings = [];
            if (spawnData.Color != null)
                colorStrings.Add(spawnData.Color);
            if (spawnData.ColorList != null)
                colorStrings.AddRange(spawnData.ColorList);

            IEnumerator<string> colorsToUse;

            if (colorStrings.Count == 0)
                colorsToUse = null;
            else
            {
                colorsToUse = FTMUtility.SelectElementsByMode(colorStrings, spawnData.ColorListMode, times).GetEnumerator();

                //move to the first color to use; if the enumerator is empty, don't use it
                //NOTE: MoveNext must be called to reach the first item when using enumerators "manually"; before this, Current is null/default
                if (!colorsToUse.MoveNext())
                    colorsToUse = null;
            }

            foreach (string monsterID in FTMUtility.SelectElementsByMode(monsterIDs, spawnData.IdListMode, times)) //for each monster to create
            {
                yield return new MonsterData()
                {
                    SpawnId = monsterID,
                    Sprite = spawnData.Sprite,
                    Scale = spawnData.Scale,
                    HideShadow = spawnData.HideShadow,
                    MaxHealth = spawnData.MaxHealth,
                    CurrentHealth = spawnData.CurrentHealth,
                    Damage = spawnData.Damage,
                    Defense = spawnData.Defense,
                    DodgeChance = spawnData.DodgeChance,
                    Experience = spawnData.Experience,
                    SightRange = spawnData.SightRange,
                    FocusedOnFarmers = spawnData.FocusedOnFarmers,
                    FacingDirection = spawnData.FacingDirection,
                    LootData = spawnData.LootData,
                    DisableExtraLoot = spawnData.DisableExtraLoot,
                    DisableInstantKill = spawnData.DisableInstantKill,
                    DisableStun = spawnData.DisableStun,
                    DaysUntilRemoved = spawnData.DaysUntilRemoved,

                    Color = colorsToUse?.Current ?? null,
                    DisableRangedAttacks = spawnData.DisableRangedAttacks,
                    Gender = spawnData.Gender,
                    Segments = spawnData.Segments,

                    ModData = spawnData.ModData
                };

                if (colorsToUse != null && !colorsToUse.MoveNext()) //if out of colors
                    colorsToUse = FTMUtility.SelectElementsByMode(colorStrings, spawnData.ColorListMode, times).GetEnumerator(); //get more with the same logic as above
            }
        }
    }
}
