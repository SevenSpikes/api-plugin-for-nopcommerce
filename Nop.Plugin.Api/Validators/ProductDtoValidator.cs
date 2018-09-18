using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class ProductDtoValidator : BaseDtoValidator<ProductDto>
    {

        #region Constructors

        public ProductDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper) : base(httpContextAccessor, jsonHelper)
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