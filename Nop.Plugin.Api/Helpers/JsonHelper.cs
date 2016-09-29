using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Nop.Plugin.Api.Helpers
{
    // source - http://stackoverflow.com/questions/5546142/how-do-i-use-json-net-to-deserialize-into-nested-recursive-dictionary-and-list
    public class JsonHelper : IJsonHelper
    {
        public Dictionary<string, object> DeserializeToDictionary(string json)
        {
            //TODO: JToken.Parse could throw an exeption if not valid JSON string is passed
            try
            {
                return ToObject(JToken.Parse(json)) as Dictionary<string, object>;
            }
            catch (Exception)
            {
                return null;
            } 
        }

        private object ToObject(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    return token.Children<JProperty>()
                                .ToDictionary(prop => prop.Name,
                                              prop => ToObject(prop.Value));

                case JTokenType.Array:
                    return token.Select(ToObject).ToList();

                default:
                    return ((JValue)token).Value;
            }
        }
    }
}