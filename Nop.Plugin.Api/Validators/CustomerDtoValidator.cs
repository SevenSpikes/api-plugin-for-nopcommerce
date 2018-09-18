using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;
using System.Net.Http;

namespace Nop.Plugin.Api.Validators
{
    public class CustomerDtoValidator : BaseDtoValidator<CustomerDto>
    {

        #region Private Fields

        private readonly ICustomerRolesHelper _customerRolesHelper;

        #endregion
        
        #region Constructors

        public CustomerDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, ICustomerRolesHelper customerRolesHelper) : base(httpContextAccessor, jsonHelper)
        {
            _customerRolesHelper = customerRolesHelper;

            SetEmailRule();
            SetRolesRule();
            SetPasswordRule();
        }

        #endregion

        #region Private Methods

        private void SetEmailRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Email, "invalid email", "email");
        }

        private void SetPasswordRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(c => c.Password, "invalid password", "password");
        }

        private void SetRolesRule()
        {
            if (HttpMethod == HttpMethod.Post || JsonDictionary.ContainsKey("role_ids"))
            {
                IList<CustomerRole> customerRoles = null;

                RuleFor(x => x.RoleIds)
                    .NotNull()
                    .Must(roles => roles.Count > 0)
                    .WithMessage("role_ids required")
                    .DependentRules(() => RuleFor(dto => dto.RoleIds)
                    .Must(roleIds =>
                    {
                        if (customerRoles == null)
                        {
                            customerRoles = _customerRolesHelper.GetValidCustomerRoles(roleIds);
                        }

                        var isInGuestAndRegisterRoles = _customerRolesHelper.IsInGuestsRole(customerRoles) &&
                                                        _customerRolesHelper.IsInRegisteredRole(customerRoles);

                    // Customer can not be in guest and register roles simultaneously
                    return !isInGuestAndRegisterRoles;
                    })
                    .WithMessage("must not be in guest and register roles simultaneously")
                    .DependentRules(() => RuleFor(dto => dto.RoleIds)
                        .Must(roleIds =>
                        {
                            if (customerRoles == null)
                            {
                                customerRoles = _customerRolesHelper.GetValidCustomerRoles(roleIds);
                            }

                            var isInGuestOrRegisterRoles = _customerRolesHelper.IsInGuestsRole(customerRoles) ||
                                                            _customerRolesHelper.IsInRegisteredRole(customerRoles);

                        // Customer must be in either guest or register role.
                        return isInGuestOrRegisterRoles;
                        })
                        .WithMessage("must be in guest or register role")
                    )
                );
            }
        }

        #endregion

    }
}