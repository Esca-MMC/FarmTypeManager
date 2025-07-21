using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>Settings only intended to test the custom action system.</summary>
    public class TestSettings
    {
        public string Name { get; set; } = "default value";
        public string OtherName { get; set; } = null;
        public int x { get; set; } = 77777;

        public bool Fake_setting_1 = false;
        public string Fake_setting_2 = null;
        public int Fake_setting_3 = 0;
        public List<string> Fake_setting_4 = null;
        public Dictionary<string, string> Fake_setting_5 = null;
    }
}
