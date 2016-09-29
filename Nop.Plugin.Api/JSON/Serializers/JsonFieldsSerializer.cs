using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Serializers
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        private const string DateTimeIso8601Format = "yyyy-MM-ddTHH:mm:ssZ";

        public string Serialize(ISerializableObject objectToSerialize, string jsonFields)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("objectToSerialize");
            }

            IList<string> fieldsList = null;

            if (!string.IsNullOrEmpty(jsonFields))
            {
                string primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();

                fieldsList = GetPropertiesIntoList(jsonFields);

                // Always add the root manually
                fieldsList.Add(primaryPropertyName);
            }

            string json = Serialize(objectToSerialize, fieldsList);

            return json;
        }

        private string Serialize(object objectToSerialize, IList<string> jsonFields = null)
        {
            var serializer = new JsonSerializer
            {
                DateFormatString = DateTimeIso8601Format
            };

            JToken jToken = JToken.FromObject(objectToSerialize, serializer);

            if (jsonFields != null)
            {
                jToken = jToken.RemoveEmptyChildrenAndFilterByFields(jsonFields);
            }

            string jTokenResult = jToken.ToString();

            return jTokenResult;
        }

        private IList<string> GetPropertiesIntoList(string fields)
        {
            IList<string> properties = fields.ToLowerInvariant()
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }
    }
}