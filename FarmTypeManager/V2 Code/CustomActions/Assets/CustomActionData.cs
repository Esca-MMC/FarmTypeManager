using Newtonsoft.Json;

namespace FarmTypeManager.CustomActions
{
    [JsonConverter(typeof(CustomActionDataJsonConverter))]
    /// <summary>A set of generic data used to perform a custom action.</summary>
    public class CustomActionData
    {
        /// <summary>The ID of the action to perform.</summary>
        public string ActionId { get; set; } = null;
        /// <summary>A <see cref="GameStateQuery"/> condition. If it's null or it returns true when checked, this entry can be performed and/or included when actions are selected.</summary>
        public string Condition { get; set; } = null;
        /// <summary>The weight to use for this entry when randomly selecting actions, if applicable.</summary>
        /// <remarks>This is equivalent to adding additional copies of this entry to a set. For example, an entry with weight = 10 should be 10 times as likely to be selected as an entry with weight = 1.</remarks>
        public int Weight { get; set; } = 1;
        /// <summary>The settings to use for when performing this action. Its type should be determined by its action ID's handler, generally at creation time.</summary>
        public object Settings { get; set; } = null;
    }
}
