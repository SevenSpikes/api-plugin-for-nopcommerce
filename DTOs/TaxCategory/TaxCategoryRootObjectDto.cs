using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Api.DTO.TaxCategory
{
    public class TaxCategoryRootObjectDto : ISerializableObject
    {
        public TaxCategoryRootObjectDto()
        {
            TaxCategories = new List<TaxCategoryDto>();
        }

        [JsonProperty("tax_categories")]
        public IList<TaxCategoryDto> TaxCategories { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "tax_categories";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof(TaxCategoryDto);
        }
    }
}
