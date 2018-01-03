using FluentValidation.Attributes;
using Newtonsoft.Json;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.ProductAttributes
{
    [JsonObject(Title = "product_attribute")]
    [Validator(typeof(ProductAttributeDtoValidator))]
    public class ProductAttributeDto
    {
        /// <summary>
        /// Gets or sets the product attribute id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        ///// <summary>
        ///// Gets or sets the localized names
        ///// </summary>
        //[JsonProperty("localized_names")]
        //public List<LocalizedNameDto> LocalizedNames
        //{
        //    get
        //    {
        //        return _localizedNames;
        //    }
        //    set
        //    {
        //        _localizedNames = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}