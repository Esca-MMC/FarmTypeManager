using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for forage generation, providing the ability to override each area's seasonal forage items
        private class ForageSpawnArea : SpawnArea
        {
            //this subclass was added in version 1.2; defaults are used here to automatically fill it in with SMAPI's json interface

            public object[] SpringItemIndex { get; set; } = null;
            public object[] SummerItemIndex { get; set; } = null;
            public object[] FallItemIndex { get; set; } = null;
            public object[] WinterItemIndex { get; set; } = null;

            public ForageSpawnArea()
                : base()
            {

            }

            public ForageSpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra, object[] spring, object[] summer, object[] fall, object[] winter)
                : base(id, name, min, max, types, include, exclude, safety, extra) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
            {
                SpringItemIndex = spring;
                SummerItemIndex = summer;
                FallItemIndex = fall;
                WinterItemIndex = winter;
            }
        }
    }
}