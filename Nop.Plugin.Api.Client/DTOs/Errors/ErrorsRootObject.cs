using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Errors
{
    public class ErrorsRootObject
    {
        [JsonProperty("errors")]
        public Dictionary<string, List<string>> Errors { get; set; }

    }
}