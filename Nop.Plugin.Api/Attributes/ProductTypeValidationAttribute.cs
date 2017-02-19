using System;
using System.Collections.Generic;
using System.Globalization;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DTOs.Images;

namespace Nop.Plugin.Api.Attributes
{
    public class ProductTypeValidationAttribute : BaseValidationAttribute
    {
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public override void Validate(object instance)
        {
            // Product Type is not required so it could be null
            // and there is nothing to validate in this case
            if (instance == null)
                return;

            ProductType productType;

            var isDefined = Enum.IsDefined(typeof(ProductType), instance);

            if (!isDefined)
            { 
                _errors.Add("ProductType","Invalid product type");
            }
        }

        public override Dictionary<string, string> GetErrors()
        {
            return _errors;
        }
    }
}