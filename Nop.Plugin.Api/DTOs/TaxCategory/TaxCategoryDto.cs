using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation.Attributes;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.DTOs.TaxCategory
{
    [JsonObject(Title = "taxcategory")]
    [Validator(typeof(TaxCategoryDtoValidator))]
    public class TaxCategoryDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the tax rate
        /// </summary>
        public decimal Rate { get; set; }
    }
}
