﻿using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.AutoMapper;
using Nop.Plugin.Api.DTO.CustomerRoles;

namespace Nop.Plugin.Api.MappingExtensions
{
    public static class CustomerRoleDtoMappings
    {
        public static CustomerRoleDto ToDto(this CustomerRole customerRole)
        {
            return customerRole.MapTo<CustomerRole, CustomerRoleDto>();
        }
    }
}
