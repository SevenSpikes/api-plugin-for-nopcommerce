using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.Validators
{
    public class ShoppingCartItemDtoValidator : AbstractValidator<ShoppingCartItemDto>
    {
        public ShoppingCartItemDtoValidator(string httpMethod, IReadOnlyDictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetCustomerIdRule();

                SetProductIdRule();

                SetQuantityRule();
                
                ValidateShoppingCartType();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(x => x.Id)
                        .NotNull()
                        .NotEmpty()
                        .Must(id => int.TryParse(id, out var parsedId) && parsedId > 0)
                        .WithMessage("Invalid Id");

                if (passedPropertyValuePaires.ContainsKey("customer_id"))
                {
                    SetCustomerIdRule();
                }

                if (passedPropertyValuePaires.ContainsKey("product_id"))
                {
                    SetProductIdRule();
                }

                if (passedPropertyValuePaires.ContainsKey("quantity"))
                {
                    SetQuantityRule();
                }

                if (passedPropertyValuePaires.ContainsKey("shopping_cart_type"))
                {
                    ValidateShoppingCartType();
                }
            }

            if (passedPropertyValuePaires.ContainsKey("rental_start_date_utc") || passedPropertyValuePaires.ContainsKey("rental_end_date_utc"))
            {
                RuleFor(x => x.RentalStartDateUtc)
                    .NotNull()
                    .WithMessage("Please provide a rental start date");

                RuleFor(x => x.RentalEndDateUtc)
                   .NotNull()
                   .WithMessage("Please provide a rental end date");

                RuleFor(dto => dto)
                    .Must(dto => dto.RentalStartDateUtc < dto.RentalEndDateUtc)
                    .WithMessage("Rental start date should be before rental end date");

                RuleFor(dto => dto)
                    .Must(dto => dto.RentalStartDateUtc > dto.CreatedOnUtc)
                    .WithMessage("Rental start date should be the future date");

                RuleFor(dto => dto)
                   .Must(dto => dto.RentalEndDateUtc > dto.CreatedOnUtc)
                   .WithMessage("Rental end date should be the future date");
            }
        }

        private void SetCustomerIdRule()
        {
            RuleFor(x => x.CustomerId)
                   .NotNull()
                   .WithMessage("Please, set customer id");
        }

        private void SetProductIdRule()
        {
            RuleFor(x => x.ProductId)
                   .NotNull()
                   .WithMessage("Please, set product id");
        }

        private void SetQuantityRule()
        {
            RuleFor(x => x.Quantity)
                      .NotNull()
                      .WithMessage("Please, set quantity");
        }

        private void ValidateShoppingCartType()
        {
            RuleFor(x => x.ShoppingCartType)
                .NotNull()
                .Must(x =>
                {
                    var parsed = Enum.TryParse(x, true, out ShoppingCartType _);
                    return parsed;
                })
                .WithMessage("Please provide a valid shopping cart type");
        }
    }
}