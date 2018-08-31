using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Plugin.Api.DTOs.Orders;

namespace Nop.Plugin.Api.Validators
{
    public class OrderDtoValidator : AbstractValidator<OrderDto>
    {
        public OrderDtoValidator(string httpMethod, IReadOnlyDictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) ||
                httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetCustomerIdRule();
            }
            else if(httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(x => x.Id)
                        .NotNull()
                        .NotEmpty()
                        .Must(id => int.TryParse(id, out var parsedId) && parsedId > 0)
                        .WithMessage("Invalid id");

                if (passedPropertyValuePaires.ContainsKey("customer_id"))
                {
                    SetCustomerIdRule();
                }
            }
        }

        private void SetCustomerIdRule()
        {
            RuleFor(x => x.CustomerId)
                      .NotNull()
                      .Must(id => id > 0)
                      .WithMessage("Invalid customer_id");
        }
    }
}