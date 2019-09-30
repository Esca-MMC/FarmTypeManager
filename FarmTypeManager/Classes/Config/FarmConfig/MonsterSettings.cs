using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //contains configuration settings for spawning monsters
        private class MonsterSettings
        {
            public MonsterSpawnArea[] Areas { get; set; }
            public int[] CustomTileIndex { get; set; }

            public MonsterSettings()
            {
                Areas = new MonsterSpawnArea[] { new MonsterSpawnArea() }; //a set of "MonsterSpawnArea", describing where monsters can spawn
                CustomTileIndex = new int[0]; //an extra list of tilesheet indices, for those who want to use their own custom terrain type
            }
        }
    }
}