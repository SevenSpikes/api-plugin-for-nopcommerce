using FluentValidation.Attributes;
using Newtonsoft.Json;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.Images
{
    [ImageValidation]
    [JsonObject(Title = "image")]
    public class ImageMappingDto : ImageDto
    {
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("picture_id")]
        public int PictureId { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }
    }
}