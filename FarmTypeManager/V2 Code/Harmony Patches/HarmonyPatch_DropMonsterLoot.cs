using FarmTypeManager.CustomActions;
using FarmTypeManager.Serialization;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Internal;
using StardewValley.Monsters;
using System;

namespace FarmTypeManager.HarmonyPatches
{
    /// <summary>A Harmony patch that drops custom loot items when monsters are defeated.</summary>
    public static class HarmonyPatch_DropMonsterLoot
    {
        /***********/
        /* Methods */
        /***********/

        /// <summary>Applies this Harmony patch to the game through the provided instance, and initializes related events.</summary>
        /// <param name="harmony">This mod's Harmony instance.</param>
        public static void ApplyPatch(Harmony harmony)
        {
            FTMUtility.Helper.Events.Multiplayer.ModMessageReceived += ModMessageReceived_DropFarmhandLoot;

            FTMUtility.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_DropMonsterLoot)}\": postfixing SDV method \"GameLocation.monsterDrop\".", LogLevel.Trace);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop), [typeof(Monster), typeof(int), typeof(int), typeof(Farmer)]),
                postfix: new HarmonyMethod(typeof(HarmonyPatch_DropMonsterLoot), nameof(monsterDrop_Postfix))
            );
        }

        /// <summary>Drops custom loot items when monsters are defeated, if available in the monster's mod data.</summary>
        public static void monsterDrop_Postfix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
        {
            try
            {
                if (monster == null || !monster.modData.TryGetValue(FTMUtility.ModDataKeys.SerializerId, out string serializerId)) //if this monster doesn't have an ID used to get custom loot
                    return;

                if (Context.IsMainPlayer)
                {
                    if (MonsterSerializer.GetData(serializerId)?.LootData is ItemSpawnField lootData && lootData != null) //if the monster has loot data
                        DropLoot(__instance, lootData, x, y, who);
                }
                else //if this is a multiplayer farmhand client
                {
                    //send a message with necessary data to the host
                    ModMessageData message = new(__instance.NameOrUniqueName, serializerId, x, y, who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID);
                    FTMUtility.Helper.Multiplayer.SendMessage(message, nameof(HarmonyPatch_DropMonsterLoot), [FTMUtility.Manifest.UniqueID], [Game1.MasterPlayer.UniqueMultiplayerID]);
                }
            }
            catch (Exception ex)
            {
                FTMUtility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DropMonsterLoot)}\" has encountered an error in method \"{nameof(monsterDrop_Postfix)}\". Monsters with custom loot might not drop items. Full error message: \n{ex.ToString()}", LogLevel.Error);
            }
        }

        /// <summary>Drops loot via the multiplayer host when farmhands send a message.</summary>
        /// <remarks>
        /// The method <see cref="GameLocation.monsterDrop"/> may be called by non-host clients, e.g. when farmhands defeat a monster. This mod currently only retains monster loot data on the host.
        /// Instead of trying to drop loot, farmhands should broadcast a mod message, which will be read in this event by the host and drop the loot itself.
        /// </remarks>
        private static void ModMessageReceived_DropFarmhandLoot(object sender, StardewModdingAPI.Events.ModMessageReceivedEventArgs e)
        {
            try
            {
                if (!Context.IsMainPlayer || e.FromModID != FTMUtility.Manifest.UniqueID || e.Type != nameof(HarmonyPatch_DropMonsterLoot)) //if the current player isn't the host, or the message is unrelated
                    return;

                var message = e.ReadAs<ModMessageData>();

                if (MonsterSerializer.GetData(message.SerializerId)?.LootData is ItemSpawnField lootData && lootData != null) //if the received monster ID has loot data
                {
                    GameLocation location = FTMUtility.GetLocationIfActive(message.LocationName);
                    if (location == null)
                        return;

                    Farmer who = Game1.GetPlayer(message.FarmerId, true) ?? Game1.player; //get the provided farmer for context, if possible
                    DropLoot(location, lootData, message.X, message.Y, who);
                }
            }
            catch (Exception ex)
            {
                FTMUtility.Monitor.LogOnce($"Harmony patch \"{nameof(HarmonyPatch_DropMonsterLoot)}\" has encountered an error in method \"{nameof(ModMessageReceived_DropFarmhandLoot)}\". Monsters with custom loot might not drop items. Full error message: \n{ex.ToString()}", LogLevel.Error);
            }
        }

        /// <summary>Drops loot items, based on the provided monster data and context.</summary>
        /// <param name="location">The monster's location.</param>
        /// <param name="lootData">The monster's custom loot data.</param>
        /// <param name="x">The horizontal origin position of any dropped items.</param>
        /// <param name="y">The vertical origin position of any dropped items.</param>
        /// <param name="who">The player to use in query contexts, e.g. the one who defeated the monster.</param>
        private static void DropLoot(GameLocation location, ItemSpawnField lootData, int x, int y, Farmer who)
        {
            GameStateQueryContext queryContext = new(location, who, null, null, null);
            ItemQueryContext itemContext = new(queryContext.Location, queryContext.Player, queryContext.Random, "FTM > HarmonyPatch_DropMonsterLoot");

            int times = lootData.GetRandomTimes(queryContext);
            if (times <= 0)
                return;

            if (FTMUtility.Monitor.IsVerbose)
                FTMUtility.Monitor.VerboseLog($"Dropping custom monster loot. Location: \"{location.NameOrUniqueName}\". Drop position: {x},{y}.");

            foreach (var item in lootData.CreateItems(queryContext, itemContext, times)) //generate items to drop
                Game1.createItemDebris(item, new Vector2(x, y), Game1.random.Next(4), queryContext.Location);
        }

        /******************/
        /* Nested classes */
        /******************/

        /// <summary>The set of data used in the outer class's mod messages.</summary>
        private class ModMessageData
        {
            public ModMessageData() { }

            public ModMessageData(string locationName, string serializerId, int x, int y, long farmerId)
            {
                LocationName = locationName;
                SerializerId = serializerId;
                X = x;
                Y = y;
                FarmerId = farmerId;
            }

            public string LocationName;
            public string SerializerId;
            public int X;
            public int Y;
            public long FarmerId;
        }

    }
}
