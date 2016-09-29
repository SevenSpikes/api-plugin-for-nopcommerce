using System.Collections.Generic;

namespace Nop.Plugin.Api.Helpers
{
    public interface IJsonHelper
    {
        Dictionary<string, object> DeserializeToDictionary(string json);
    }
}