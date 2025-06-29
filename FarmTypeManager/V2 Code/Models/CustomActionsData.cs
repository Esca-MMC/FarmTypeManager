using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of data that describes when to trigger customizable actions, which actions to trigger, and settings/arguments specific to each action.</summary>
    public class CustomActionsData
    {
        public string Triggers { get; set; } = null;
        public string Condition { get; set; } = null;
        public string ActionsMode { get; set; } = "All";
        public int MinTimes { get; set; } = 1;
        public int MaxTimes { get; set; } = 1;
        public List<CustomActionHandlerData> CustomActions { get; set; }
    }
}
