using Newtonsoft.Json;

namespace FarmTypeManager.CustomActions
{
    [JsonConverter(typeof(CustomActionDataJsonConverter))]
    /// <summary>A set of generic data used to perform a custom action.</summary>
    public class CustomActionData : IWeightedConditionalElement
    {
        /// <summary>The type of custom action to perform, e.g. "SpawnObject".</summary>
        public string ActionType { get; set; } = null;

        public string Condition { get; set; } = null;
        public string MarkAppliedWithFlag { get; set; } = null;
        public int Weight { get; set; } = 1;

        /// <summary>The settings to use for when performing this action. Its type should be determined by its action type's handler, generally at creation time.</summary>
        public object Settings { get; set; } = null;
    }
}
