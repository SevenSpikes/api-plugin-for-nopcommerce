using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Nop.Plugin.Api.Client.Serialization
{
    public class NewtonsoftSerializer : ISerializer, IDeserializer
    {
        /// <summary>
        ///     Encoding to use to convert string to byte[] and the other way around.
        /// </summary>
        /// <remarks>
        ///     StackExchange.Redis uses Encoding.UTF8 to convert strings to bytes,
        ///     hence we do same here.
        /// </remarks>
        private static readonly Encoding s_encoding = Encoding.UTF8;

        private readonly JsonSerializer _serializer;

        private readonly JsonSerializerSettings _settings;

        public NewtonsoftSerializer()
        {
            _serializer = new JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };

            _settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        public NewtonsoftSerializer(JsonSerializer serializer, JsonSerializerSettings settings)
        {
            _serializer = serializer;
            _settings = settings;
        }

        public T Deserialize<T>(IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }

        public byte[] Serialize(object item)
        {
            var jsonString = JsonConvert.SerializeObject(item, _settings);
            return s_encoding.GetBytes(jsonString);
        }

        public async Task<byte[]> SerializeAsync(object item)
        {
            var jsonString = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(item, _settings));

            return s_encoding.GetBytes(jsonString);
        }

        public object Deserialize(byte[] serializedObject)
        {
            var jsonString = s_encoding.GetString(serializedObject);
            return JsonConvert.DeserializeObject(jsonString, typeof(object), _settings);
        }

        public Task<object> DeserializeAsync(byte[] serializedObject)
        {
            return Task.Factory.StartNew(() => Deserialize(serializedObject));
        }

        public T Deserialize<T>(byte[] serializedObject)
            where T : class
        {
            var jsonString = s_encoding.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString, _settings);
        }

        public Task<T> DeserializeAsync<T>(byte[] serializedObject)
            where T : class
        {
            return Task.Factory.StartNew(() => Deserialize<T>(serializedObject));
        }

        string ISerializer.Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                var jsonTextWriter =
                    new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented, QuoteChar = '"' };

                _serializer.Serialize(jsonTextWriter, obj);

                var result = stringWriter.ToString();
                return result;
            }
        }

        /// <summary>
        ///     Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        ///     Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        ///     Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Content type for serialized content
        /// </summary>
        public string ContentType
        {
            get => "application/json";
            set
            {
            }
        }
    }
}