using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;

namespace Nop.Plugin.Api.Validators
{
    public class ProductCategoryMappingDtoValidator : AbstractValidator<ProductCategoryMappingDto>
    {
        public ProductCategoryMappingDtoValidator(string httpMethod, IReadOnlyDictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(mapping => mapping.CategoryId)
                    .Must(categoryId => categoryId > 0)
                    .WithMessage("invalid category_id")
                    .DependentRules(() =>
                    {
                        RuleFor(a => a.ProductId)
                            .Must(productId => productId > 0)
                            .WithMessage("invalid product_id");
                    });
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(mapping => mapping.Id)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => id > 0)
                    .WithMessage("invalid id");

                if (passedPropertyValuePaires.ContainsKey("category_id"))
                {
                    RuleFor(mapping => mapping.CategoryId)
                        .Must(categoryId => categoryId > 0)
                        .WithMessage("category_id invalid");
                }

                if (passedPropertyValuePaires.ContainsKey("product_id"))
                {
                    RuleFor(mapping => mapping.ProductId)
                        .Must(productId => productId > 0)
                        .WithMessage("product_id invalid");
                }
            }
        }
    }
}
