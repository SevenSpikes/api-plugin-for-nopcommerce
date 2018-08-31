using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.Images
{
    public class ImageMappingDto : ImageDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }
    }
}