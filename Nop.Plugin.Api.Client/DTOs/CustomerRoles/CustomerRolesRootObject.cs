using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Client.DTOs.CustomerRoles
{
    public class CustomerRolesRootObject
    {
        public CustomerRolesRootObject()
        {
            CustomerRoles = new List<CustomerRoleDto>();
        }

        [JsonProperty("customer_roles")]
        public IList<CustomerRoleDto> CustomerRoles { get; set; }

    }
}
