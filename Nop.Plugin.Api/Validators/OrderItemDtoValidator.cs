using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Plugin.Api.DTOs.OrderItems;

namespace Nop.Plugin.Api.Validators
{
    public class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
    {
        public OrderItemDtoValidator(string httpMethod, IReadOnlyDictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) ||
                httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetProductRule();
                SetQuantityRule();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                if (passedPropertyValuePaires.ContainsKey("product_id"))
                {
                    SetProductRule();
                }

                if (passedPropertyValuePaires.ContainsKey("quantity"))
                {
                    SetQuantityRule();
                }
            }
        }

        private void SetProductRule()
        {
            RuleFor(x => x.ProductId)
                    .NotNull()
                    .WithMessage("Invalid product id");
        }

        private void SetQuantityRule()
        {
            RuleFor(x => x.Quantity)
                    .NotNull()
                    .Must(quantity => quantity > 0)
                    .WithMessage("Invalid quantity");
        }
    }
}