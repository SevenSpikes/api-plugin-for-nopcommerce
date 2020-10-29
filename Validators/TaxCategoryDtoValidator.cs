using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTO.Products;
using Nop.Plugin.Api.DTO.TaxCategory;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class TaxCategoryDtoValidator : BaseDtoValidator<TaxCategoryDto>
    {
        #region Constructors

        public TaxCategoryDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.Name, "invalid name", "name");
        }

        #endregion
    }
}
