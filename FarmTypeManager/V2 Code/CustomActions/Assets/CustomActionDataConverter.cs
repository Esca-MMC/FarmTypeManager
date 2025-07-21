using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FarmTypeManager.CustomActions
{
    /// <summary>The Newtonsoft JSON converter used by <see cref="CustomActionData"/>.</summary>
    /// <remarks>
    /// <para>This converter causes the "Settings" field to deserialize into a variable type based on the value of "ActionId".</para>
    /// <para>This is intended to allow the mod Content Patcher to interact with "Settings" like a strongly typed field, e.g. to edit nested field values, while also letting each action define its own fields.</para>
    /// <para>Notably, some of Content Patcher's features are not fully compatible with nested generic fields or JsonExtensionData.</para>
    /// </remarks>
    public class CustomActionDataJsonConverter : JsonConverter<CustomActionData>
    {
        public override CustomActionData ReadJson(JsonReader reader, Type objectType, CustomActionData existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var rawValue = JObject.Load(reader);

            if (rawValue is null || rawValue.Type is JTokenType.Null)
                return null;

            CustomActionData newValue = new();

            //read all non-custom fields' values
            if (rawValue.TryGetValue("ActionId", StringComparison.OrdinalIgnoreCase, out JToken actionId) && actionId != null)
                newValue.ActionId = actionId.ToObject<string>(serializer);
            if (rawValue.TryGetValue("Condition", StringComparison.OrdinalIgnoreCase, out JToken condition) && condition != null)
                newValue.Condition = condition?.ToObject<string>(serializer);
            if (rawValue.TryGetValue("Weight", StringComparison.OrdinalIgnoreCase, out JToken weight) && weight != null)
                newValue.Weight = weight.ToObject<int>(serializer);

            //read Settings into a type that varies by ActionId (NOTE: this enables Content Patcher Fields/TargetField support for action-specific fields)
            if (rawValue.TryGetValue("Settings", StringComparison.OrdinalIgnoreCase, out JToken settings))
            {
                if (settings.Type == JTokenType.Null)
                    newValue.Settings = null;
                else if (newValue.ActionId != null && CustomActionManager.GetSettingsType(newValue.ActionId) is Type settingsType)
                    newValue.Settings = settings.ToObject(settingsType, serializer);
                else
                    newValue.Settings = settings.ToObject<Dictionary<string, object>>(serializer);
            }

            return newValue;
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, CustomActionData value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
