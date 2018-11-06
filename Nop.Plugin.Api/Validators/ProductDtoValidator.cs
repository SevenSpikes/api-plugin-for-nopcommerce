using FluentValidation;
using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.Services;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class ProductDtoValidator : BaseDtoValidator<ProductDto>
    {

        #region Private Fields

        private readonly IProductApiService _productApiService;

        #endregion

        #region Constructors

        public ProductDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary, IProductApiService productApiService) : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            _productApiService = productApiService;

            SetNameRule();

            RuleFor(product => product.Sku)
                .Must(SkuMustBeUniqueForCreation)
                .WithMessage("sku must be unique");

            When(product => !product.MarkAsNew, () => {
                RuleFor(product => product.MarkAsNewStartDateTimeUtc)
                    .Null()
                    .WithMessage("Mark as start date cannot be set with not market as new");

                RuleFor(product => product.MarkAsNewEndDateTimeUtc)
                    .Null()
                    .WithMessage("Mark as end date cannot be set with not market as new");
            });

        }

        #endregion

        #region Private Methods

        private void SetNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(p => p.Name, "invalid name", "name");
        }

        private bool SkuMustBeUniqueForCreation(string sku)
        {
            if (this.HttpMethod != System.Net.Http.HttpMethod.Post)
            {
                return true;
            }

            if (string.IsNullOrEmpty(sku)) return true;

            return !_productApiService.ProductSkuExists(sku);
        }

        #endregion

    }
}