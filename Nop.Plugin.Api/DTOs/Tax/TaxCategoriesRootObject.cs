using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Tax
{
    public class TaxCategoriesRootObject : ISerializableObject
    {
        public TaxCategoriesRootObject()
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
