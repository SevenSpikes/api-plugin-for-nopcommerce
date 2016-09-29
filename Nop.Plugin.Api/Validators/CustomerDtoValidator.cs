using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        private ICustomerRolesHelper _customerRolesHelper = EngineContext.Current.Resolve<ICustomerRolesHelper>();

        public CustomerDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) ||
                httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetRuleForRoles();
                SetRuleForEmail();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                int parsedId = 0;

                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => int.TryParse(id, out parsedId) && parsedId > 0)
                    .WithMessage("invalid id");

                if (passedPropertyValuePaires.ContainsKey("email"))
                {
                    SetRuleForEmail();
                }

                // TODO: think of a way to not hardcode the json property name.
                if (passedPropertyValuePaires.ContainsKey("role_ids"))
                {
                    SetRuleForRoles();
                }
            }

            if (passedPropertyValuePaires.ContainsKey("password"))
            {
                RuleForEach(customer => customer.Password)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("invalid password");
            }

            // The fields below are not required, but if they are passed they should be validated.
            if (passedPropertyValuePaires.ContainsKey("billing_address"))
            {
                RuleFor(x => x.BillingAddress)
                    .SetValidator(new AddressDtoValidator());
            }

            if (passedPropertyValuePaires.ContainsKey("shipping_address"))
            {
                RuleFor(x => x.ShippingAddress)
                    .SetValidator(new AddressDtoValidator());
            }

            if (passedPropertyValuePaires.ContainsKey("addresses"))
            {
                RuleForEach(x => x.CustomerAddresses)
                    .SetValidator(new AddressDtoValidator());
            }
        }

        private void SetRuleForEmail()
        {
            RuleFor(customer => customer.Email)
                .NotNull()
                .NotEmpty()
                .WithMessage("email can not be empty");
        }

        private void SetRuleForRoles()
        {
            IList<CustomerRole> customerRoles = null;

            RuleFor<List<int>>(x => x.RoleIds)
                   .NotNull()
                   .Must(roles => roles.Count > 0)
                   .WithMessage("role_ids required")
                   .DependentRules(dependentRules => dependentRules.RuleFor(dto => dto.RoleIds)
                       .Must(roleIds =>
                       {
                           if (customerRoles == null)
                           {
                               customerRoles = _customerRolesHelper.GetValidCustomerRoles(roleIds);
                           }

                           bool isInGuestAndRegisterRoles = _customerRolesHelper.IsInGuestsRole(customerRoles) &&
                                                            _customerRolesHelper.IsInRegisteredRole(customerRoles);

                           // Customer can not be in guest and register roles simultaneously
                           return !isInGuestAndRegisterRoles;
                       })
                       .WithMessage("must not be in guest and register roles simultaneously")
                       .DependentRules(dependentRule => dependentRules.RuleFor(dto => dto.RoleIds)
                            .Must(roleIds =>
                            {
                                if (customerRoles == null)
                                {
                                    customerRoles = _customerRolesHelper.GetValidCustomerRoles(roleIds);
                                }

                                bool isInGuestOrRegisterRoles = _customerRolesHelper.IsInGuestsRole(customerRoles) ||
                                                                _customerRolesHelper.IsInRegisteredRole(customerRoles);

                                // Customer must be in either guest or register role.
                                return isInGuestOrRegisterRoles;
                            })
                            .WithMessage("must be in guest or register role")
                       )
                   );
        }
    }
}