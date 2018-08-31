using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.Serialization
{
    public static class JsonSerializationUtility
    {
        public static bool TryDeserialize<T>(string json, out T result)
        {
            try
            {
                result = Deserialize<T>(json);
                return true;
            }
            catch
            {
                result = default(T);
            }

            return false;
        }
        
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object Deserialize(string json, Type objectType)
        {
            return JsonConvert.DeserializeObject(json, objectType);
        }

        public static string Serialize<T>(T obj, bool indent = false, JsonConverter converter = null)
        {
            var sb = new StringBuilder();
            var settings = new JsonSerializerSettings();

            if (indent)
            {
                settings.Formatting = Formatting.Indented;
            }

            if (converter != null)
            {
                settings.Converters.Add(converter);
            }

            JsonSerializer.CreateDefault(settings).Serialize(new JsonTextWriter(new StringWriter(sb)), obj);

            return sb.ToString();
        }
    }
}