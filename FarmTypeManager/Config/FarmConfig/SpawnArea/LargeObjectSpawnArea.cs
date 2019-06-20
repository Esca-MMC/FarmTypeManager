using StardewModdingAPI;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        //a subclass of "SpawnArea" specifically for large object generation, including settings for which object types to spawn & a one-time switch to find and respawn pre-existing objects
        private class LargeObjectSpawnArea : SpawnArea
        {
            public string[] ObjectTypes { get; set; }
            public bool FindExistingObjectLocations { get; set; }
            public int PercentExtraSpawnsPerSkillLevel { get; set; }
            public string RelatedSkill { get; set; }

            public LargeObjectSpawnArea()
                : base()
            {

            }

            public LargeObjectSpawnArea(string id, string name, int min, int max, string[] types, string[] include, string[] exclude, string safety, ExtraConditions extra, string[] objTypes, bool find, int extraSpawns, string skill)
                : base(id, name, min, max, types, include, exclude, safety, extra) //uses the original "SpawnArea" constructor to fill in the shared fields as usual
            {
                ObjectTypes = objTypes;
                FindExistingObjectLocations = find;
                PercentExtraSpawnsPerSkillLevel = extraSpawns;
                RelatedSkill = skill;
            }
        }
    }
}