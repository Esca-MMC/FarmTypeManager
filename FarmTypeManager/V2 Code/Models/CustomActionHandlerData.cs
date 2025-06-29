using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>A set of generic data that can be used to create a <see cref="ICustomActionHandler"/>.</summary>
    public class CustomActionHandlerData
    {
        public string Id { get; set; } = null;
        public string Action { get; set; } = null;
        public string Condition { get; set; } = null;
        public int Weight { get; set; } = 1;
        public Dictionary<string, JToken> Settings { get; set; } = null;
    }
}
