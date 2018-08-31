using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs
{
    [JsonObject(Title = "attribute")]
    public class ProductItemAttributeDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
