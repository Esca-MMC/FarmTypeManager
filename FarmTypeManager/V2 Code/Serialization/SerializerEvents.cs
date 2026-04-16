using FarmTypeManager.Utilities;

namespace FarmTypeManager.Serialization
{
    public static class SerializerEvents
    {
        /// <summary>Initalize serialization-related events.</summary>
        public static void Initialize()
        {
            Properties.Helper.Events.GameLoop.GameLaunched += (_, _) => GameLaunched_Initialize();
        }

        private static void GameLaunched_Initialize()
        {
            MonsterSerializer.Initialize();
            PlacedItemSerializer.Initialize();
        }
    }
}
