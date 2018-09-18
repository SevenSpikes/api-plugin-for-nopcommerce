using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.ProductAttributes;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class ProductAttributeDtoValidator : BaseDtoValidator<ProductAttributeDto>
    {

        #region Constructors

        public ProductAttributeDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper) : base(httpContextAccessor, jsonHelper)
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