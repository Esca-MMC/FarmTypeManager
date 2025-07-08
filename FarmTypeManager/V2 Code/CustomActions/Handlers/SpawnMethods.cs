using StardewValley;

namespace FarmTypeManager
{
    /// <summary>A set of methods that spawn various in-game instances.</summary>
    public static class SpawnMethods
    {
        /// <summary>Spawn an <see cref="Object"/> and place it on a tile.</summary>
        /// <param name="args">A set of arguments used by this method.</param>
        /// <returns>True if an object was successfully created and placed. False if something prevented placement.</returns>
        public static bool SpawnObject(SpawnObjectArgs args)
        {
            if (args.GameLocation == null || args.Tile == null)
                return false;

            //create the object

            Item rawSpawn = null;

            if (args.Preserve.HasValue)
                rawSpawn = Utility.CreateFlavoredItem(args.Preserve.Value.ToString(), args.SpawnId); //create a preserve item, use the spawn ID as the ingredient
            else
                rawSpawn = ItemRegistry.Create(args.SpawnId, 1, 0, true); //create a normal item, or null if the data ID is invalid

            if (rawSpawn is not Object spawn)
                return false;

            //apply optional settings to the object

            if (args.AddModData != null)
            {
                foreach (var entry in args.AddModData)
                    spawn.modData[entry.Key] = entry.Value;
            }

            if (args.CanPickUp.HasValue)
                spawn.IsSpawnedObject = args.CanPickUp.Value;
            else
                spawn.IsSpawnedObject = FTMUtility.CanPickUpByDefault(spawn.ItemId); //use this method to check hard-coded values; the game does not provide data on which objects should/shouldn't be picked up

            if (args.Flipped.HasValue)
                spawn.Flipped = args.Flipped.Value;

            if (args.Fragility.HasValue)
                spawn.Fragility = args.Fragility.Value;

            if (args.Health.HasValue) //a.k.a. durability, minutes until ready
                spawn.MinutesUntilReady = args.Health.Value;

            //place the object

            spawn.Location = args.GameLocation;
            spawn.TileLocation = args.Tile.Value;

            return args.GameLocation.objects.TryAdd(args.Tile.Value, spawn);
        }
    }
}
