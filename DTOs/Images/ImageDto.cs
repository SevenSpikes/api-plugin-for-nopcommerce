using Newtonsoft.Json;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Base;

namespace Nop.Plugin.Api.DTO.Images
{
    [ImageValidation]
    public class ImageDto : BaseDto
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("attachment")]
        public string Attachment { get; set; }

        [JsonIgnore]
        public byte[] Binary { get; set; }

        [JsonIgnore]
        public string MimeType { get; set; }

        [JsonProperty("seoFilename")]
        public string SeoFilename { get; set; }
    }
}