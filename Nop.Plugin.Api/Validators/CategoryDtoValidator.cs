using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class CategoryDtoValidator : BaseDtoValidator<CategoryDto>
    {

        #region Constructors

        public CategoryDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper) : base(httpContextAccessor, jsonHelper)
        {
            SetNameRule();
        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Name, "invalid name", "name");
        }

        #endregion

    }
}