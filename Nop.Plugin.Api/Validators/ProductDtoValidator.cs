using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Plugin.Api.DTOs.Products;

namespace Nop.Plugin.Api.Validators
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator(string httpMethod, IReadOnlyDictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetNameRule();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(x => x.Id)
                        .NotNull()
                        .NotEmpty()
                        .Must(id => int.TryParse(id, out var parsedId) && parsedId > 0)
                        .WithMessage("invalid id");

                if (passedPropertyValuePaires.ContainsKey("name"))
                {
                    SetNameRule();
                }
            }
        }

        private void SetNameRule()
        {
            RuleFor(x => x.Name)
                       .NotNull()
                       .NotEmpty()
                       .WithMessage("name is required");
        }
    }
}