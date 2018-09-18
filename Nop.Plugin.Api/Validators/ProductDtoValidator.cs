using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class ProductDtoValidator : BaseDtoValidator<ProductDto>
    {
        public ProductDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper) : base(httpContextAccessor, jsonHelper)
        {
            if (HttpMethods.IsPost(httpContextAccessor.HttpContext.Request.Method))
            {
                SetNameRule(false);
            }
            else if (HttpMethods.IsPut(httpContextAccessor.HttpContext.Request.Method))
            {
                SetRequiredIdRule();
                SetNameRule(true);
            }
        }

        private void SetNameRule(bool isJsonValueRequiredForValidation)
        {
            if (!isJsonValueRequiredForValidation || JsonDictionary.ContainsKey("name"))
            {
                RuleFor(x => x.Name)
                           .NotNull()
                           .NotEmpty()
                           .WithMessage("name is required");
            }
        }
    }
}