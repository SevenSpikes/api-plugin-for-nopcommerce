using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Languages
{
    public class LanguagesRootObject
    {
        public LanguagesRootObject()
        {
            Languages = new List<LanguageDto>();
        }

        [JsonProperty("languages")]
        public IList<LanguageDto> Languages { get; set; }
        
    }
}
