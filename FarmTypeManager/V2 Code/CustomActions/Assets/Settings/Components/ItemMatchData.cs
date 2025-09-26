using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of criteria that may match spawned in-game items, used to filter or compare them.</summary>
    public class ItemMatchData
    {
        /*******************/
        /* Item properties */
        /*******************/

        public int? Category { get; set; } = null;

        public List<int> CategoryList { get; set; } = null;

        public string ContextTag { get; set; } = null;

        public List<string> ContextTagList { get; set; } = null;

        public int? Fragility { get; set; } = null;

        public string Id { get; set; } = null;

        public List<string> IdList { get; set; } = null;

        public string Name { get; set; } = null;

        public List<string> NameList { get; set; } = null;

        public int? MinMinutesUntilReady { get; set; } = null;

        public int? MaxMinutesUntilReady { get; set; } = null;

        public int? MinStackSize { get; set; } = null;

        public int? MaxStackSize { get; set; } = null;

        public int? MinQuality { get; set; } = null;

        public int? MaxQuality { get; set; } = null;

        public Dictionary<string, string> ModData { get; set; } = null;

        public string PerItemCondition { get; set; } = null;

        public string Type { get; set; } = null;

        public List<string> TypeList { get; set; } = null;
    }
}
