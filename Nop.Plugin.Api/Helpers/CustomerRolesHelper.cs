using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Constants;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Helpers
{
    public class CustomerRolesHelper : ICustomerRolesHelper
    {
        private const string CUSTOMERROLES_ALL_KEY = "Nop.customerrole.all-{0}";

        private readonly ICustomerService _customerService;
        private readonly ICacheManager _cacheManager;

        public CustomerRolesHelper(ICustomerService customerService, ICacheManager cacheManager)
        {
            _customerService = customerService;
            _cacheManager = cacheManager;
        }

        public IList<CustomerRole> GetValidCustomerRoles(List<int> roleIds)
        {
            // This is needed because the caching messeup the entity framework context
            // and when you try to send something TO the database it throws an exeption.
            _cacheManager.RemoveByPattern(CUSTOMERROLES_ALL_KEY);

            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
            {
                if (roleIds != null && roleIds.Contains(customerRole.Id))
                {
                    newCustomerRoles.Add(customerRole);
                }
            }

            return newCustomerRoles;
        }

        public bool IsInGuestsRole(IList<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests) != null;
        }

        public bool IsInRegisteredRole(IList<CustomerRole> customerRoles)
        {
            return customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Registered) != null;
        }

        public int getCustomerStore(Customer customer)
        {
            if (customer.IsInCustomerRole(BLBSettings.TradeCustomerRoleSystemName))
            {
                return BLBSettings.BigMamaStoreid;
            }

            return BLBSettings.BLBStoreId;
        }

        public int getCustomerStoreByRoleIds(List<int> roleIds)
        {
            var isInTraceRole = false;

            foreach(var roleId in roleIds)
            {
                CustomerRole customerRole = _customerService.GetCustomerRoleById(roleId);

                if (customerRole.SystemName.Equals(BLBSettings.TradeCustomerRoleSystemName))
                {
                    isInTraceRole = true;

                    break;
                }
            }

            if (isInTraceRole)
            {
                return BLBSettings.BigMamaStoreid;
            }

            return BLBSettings.BLBStoreId;
        }
    }
}