using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //contains configuration settings for spawning large objects (e.g. stumps and logs)
        private class LargeObjectSettings
        {
            public LargeObjectSpawnArea[] Areas { get; set; }
            public int[] CustomTileIndex { get; set; }

            public LargeObjectSettings()
            {
                Areas = new LargeObjectSpawnArea[] { new LargeObjectSpawnArea("", "Farm", 999, 999, new string[0], new string[0], new string[0], "High", new ExtraConditions(), new string[] { "Stump" }, true, 0, "Foraging") }; //a set of "LargeObjectSpawnArea", describing where large objects can spawn on each map
                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for use by players who want to make some custom tile detection
            }
        }
    }
}