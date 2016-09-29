using System;
using System.Linq.Expressions;
using FluentValidation;
using Nop.Plugin.Api.DTOs;

namespace Nop.Plugin.Api.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        public AddressDtoValidator()
        {
            SetNotNullOrEmptyRule(dto => dto.FirstName, "first_name required");

            SetNotNullOrEmptyRule(dto => dto.LastName, "last_name required");

            SetNotNullOrEmptyRule(dto => dto.Email, "email required");

            SetNotNullOrEmptyRule(dto => dto.CountryId <= 0 ? string.Empty : dto.CountryId.ToString(), "country_id required");

            SetNotNullOrEmptyRule(dto => dto.City, "city required");

            SetNotNullOrEmptyRule(dto => dto.Address1, "address1 required");

            SetNotNullOrEmptyRule(dto => dto.ZipPostalCode, "zip_postal_code required");

            SetNotNullOrEmptyRule(dto => dto.PhoneNumber, "phone_number required");
        }
        
        private void SetNotNullOrEmptyRule(Expression<Func<AddressDto, string>> expression, string errorMessage)
        {
            RuleFor(expression)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage(errorMessage);
        }
    }
}