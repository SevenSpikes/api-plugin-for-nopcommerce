using System;
using System.Collections.Generic;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTOs.Customers;
using System.Linq;
using System.Text.RegularExpressions;
using Nop.Core.Domain.Common;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Web.Framework.Kendoui;

namespace Nop.Plugin.Api.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string KeyGroup = "customer";

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public CustomerApiService(IRepository<Customer> customerRepository, IRepository<GenericAttribute> genericAttributeRepository)
        {
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
        }

        public IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetCustomersQuery(createdAtMin, createdAtMax, sinceId);

            IList<CustomerDto> result = HandleCustomerGenericAttributes(null, query, limit, page);

            return result;
        }

        public int GetCustomersCount()
        {
            return _customerRepository.TableNoTracking.Count(customer => !customer.Deleted);
        }

        // Need to work with dto object so we can map the first and last name from generic attributes table.
        public IList<CustomerDto> Search(string queryParams = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit)
        {
            IList<CustomerDto> result = new List<CustomerDto>();

            Dictionary<string, string> searchParams = EnsureSearchQueryIsValid(queryParams, ParseSearchQuery);

            if (searchParams != null)
            {
                IQueryable<Customer> query = _customerRepository.TableNoTracking.Where(customer => !customer.Deleted);

                foreach (var searchParam in searchParams)
                {
                    // Skip non existing properties.
                    if (ReflectionHelper.HasProperty(searchParam.Key, typeof(Customer)))
                    {
                        // @0 is a placeholder used by dynamic linq and it is used to prevent possible sql injections.
                        query = query.Where(string.Format("{0} = @0 || {0}.Contains(@0)", searchParam.Key), searchParam.Value);
                    }
                    // The code bellow will search in customer addresses as well.
                    //else if (HasProperty(searchParam.Key, typeof(Address)))
                    //{
                    //    query = query.Where(string.Format("Addresses.Where({0} == @0).Any()", searchParam.Key), searchParam.Value);
                    //}
                }

                result = HandleCustomerGenericAttributes(searchParams, query, limit, page, order);
            }

            return result;
        }

        public Dictionary<string, string> GetFirstAndLastNameByCustomerId(int customerId)
        {
            return _genericAttributeRepository.TableNoTracking.Where(
                x =>
                    x.KeyGroup == KeyGroup && x.EntityId == customerId &&
                    (x.Key == FirstName || x.Key == LastName)).ToDictionary(x => x.Key.ToLowerInvariant(), y => y.Value);
        }

        public Customer GetCustomerEntityById(int id)
        {
            Customer customer = _customerRepository.Table.FirstOrDefault(c => c.Id == id && !c.Deleted);

            return customer;
        }

        public CustomerDto GetCustomerById(int id, bool showDeleted = false)
        {
            if (id == 0)
                return null;

            // Here we expect to get two records, one for the first name and one for the last name.
            List<CustomerAttributeMappingDto> customerAttributeMappings = (from customer in _customerRepository.TableNoTracking
                                                                           join attribute in _genericAttributeRepository.TableNoTracking on customer.Id equals attribute.EntityId
                                                                           where customer.Id == id && 
                                                                                 attribute.KeyGroup.Equals(KeyGroup, StringComparison.InvariantCultureIgnoreCase) &&
                                                                                 (attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase) ||
                                                                                  attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                                                                           select new CustomerAttributeMappingDto()
                                                                           {
                                                                               Attribute = attribute,
                                                                               Customer = customer
                                                                           }).ToList();

            CustomerDto customerDto = null;

            // This is in case we have first and last names set for the customer.
            if (customerAttributeMappings.Count > 0)
            {
                // The customer object is the same in all mappings.
                customerDto = customerAttributeMappings.First().Customer.ToDto();

                foreach (var mapping in customerAttributeMappings)
                {
                    if(!showDeleted && mapping.Customer.Deleted)
                    {
                        continue;
                    }

                    if (mapping.Attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.FirstName = mapping.Attribute.Value;
                    }
                    else if (mapping.Attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.LastName = mapping.Attribute.Value;
                    }
                }
            }
            else
            {
                // This is when we do not have first and last name set.
                Customer currentCustomer = _customerRepository.TableNoTracking.FirstOrDefault(customer => customer.Id == id);

                if (currentCustomer != null)
                {
                    if (showDeleted || !currentCustomer.Deleted)
                    {
                        customerDto = currentCustomer.ToDto();
                    }
                }
            }

            return customerDto;
        }

        private Dictionary<string, string> EnsureSearchQueryIsValid(string query, Func<string, Dictionary<string, string>> parseSearchQuery)
        {
            if (!string.IsNullOrEmpty(query))
            {
                return parseSearchQuery(query);
            }

            return null;
        }

        private Dictionary<string, string> ParseSearchQuery(string query)
        {
            var parsedQuery = new Dictionary<string, string>();

            string splitPattern = @"(\w+):";

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != String.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (int i = 0; i < fieldValueList.Count; i += 2)
            {
                var field = fieldValueList[i];
                var value = fieldValueList[i + 1];

                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                {
                    field = field.Replace("_", string.Empty);
                    parsedQuery.Add(field.Trim(), value.Trim());
                }
            }

            return parsedQuery;
        }

        /// <summary>
        /// The idea of this method is to get the first and last name from the GenericAttribute table and to set them in the CustomerDto object.
        /// </summary>
        /// <param name="searchParams">Search parameters is used to shrinc the range of results from the GenericAttibutes table 
        /// to be only those with specific search parameter (i.e. currently we focus only on first and last name).</param>
        /// <param name="query">Query parameter represents the current customer records which we will join with GenericAttributes table.</param>
        /// <returns></returns>
        private IList<CustomerDto> HandleCustomerGenericAttributes(Dictionary<string, string> searchParams, IQueryable<Customer> query,
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, string order = Configurations.DefaultOrder)
        {
            // Here we join the GenericAttribute records with the customers and making sure that we are working only with the attributes
            // that are in the customers keyGroup and their keys are either first or last name.
            // We are returning a collection with customer record and attribute record. 
            // It will look something like:
            // customer data for customer 1
            //      attribute that contains the first name of customer 1
            //      attribute that contains the last name of customer 1
            // customer data for customer 2, 
            //      attribute that contains the first name of customer 2
            //      attribute that contains the last name of customer 2
            // etc.

            IQueryable<IGrouping<int, CustomerAttributeMappingDto>> allRecordsGroupedByCustomerId =
                (from customer in query
                 from attribute in _genericAttributeRepository.TableNoTracking
                     .Where(attr => attr.EntityId == customer.Id &&
                                    attr.KeyGroup.Equals(KeyGroup, StringComparison.InvariantCultureIgnoreCase) &&
                                    (attr.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase) ||
                                    attr.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))).DefaultIfEmpty()
                 select new CustomerAttributeMappingDto()
                 {
                     Attribute = attribute,
                     Customer = customer
                 }).GroupBy(x => x.Customer.Id);

            if (searchParams != null && searchParams.Count > 0)
            {
                if (searchParams.ContainsKey(FirstName))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, FirstName, searchParams[FirstName]);
                }

                if (searchParams.ContainsKey(LastName))
                {
                    allRecordsGroupedByCustomerId = GetCustomerAttributesMappingsByKey(allRecordsGroupedByCustomerId, LastName, searchParams[LastName]);
                }
            }

            IList<CustomerDto> result = GetFullCustomerDtos(allRecordsGroupedByCustomerId, page, limit, order);

            return result;
        }

        /// <summary>
        /// This method is responsible for getting customer dto records with first and last names set from the attribute mappings.
        /// </summary>
        private IList<CustomerDto> GetFullCustomerDtos(IQueryable<IGrouping<int, CustomerAttributeMappingDto>> customerAttributesMappings,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit, string order = Configurations.DefaultOrder)
        {
            var customerDtos = new List<CustomerDto>();

            customerAttributesMappings = customerAttributesMappings.OrderBy(x => x.Key);

            IList<IGrouping<int, CustomerAttributeMappingDto>> customerAttributeGroupsList = new ApiList<IGrouping<int, CustomerAttributeMappingDto>>(customerAttributesMappings, page - 1, limit);

            foreach (var group in customerAttributeGroupsList)
            {
                IList<CustomerAttributeMappingDto> mappingsForMerge = group.Select(x => x).ToList();

                CustomerDto customerDto = Merge(mappingsForMerge);

                customerDtos.Add(customerDto);
            }

            // Needed so we can apply the order parameter
            return customerDtos.AsQueryable().OrderBy(order).ToList();
        }

        private CustomerDto Merge(IList<CustomerAttributeMappingDto> mappingsForMerge)
        {
            var customerDto = new CustomerDto();

            // We expect the customer to be always set.
            customerDto = mappingsForMerge.First().Customer.ToDto();

            List<GenericAttribute> attributes = mappingsForMerge.Select(x => x.Attribute).ToList();

            foreach (var attribute in attributes)
            {
                if (attribute != null)
                {
                    if (attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.FirstName = attribute.Value;
                    }
                    else if (attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        customerDto.LastName = attribute.Value;
                    }
                }
            }

            return customerDto;
        }

        private IQueryable<IGrouping<int, CustomerAttributeMappingDto>> GetCustomerAttributesMappingsByKey(
            IQueryable<IGrouping<int, CustomerAttributeMappingDto>> customerAttributesGroups, string key, string value)
        {
            // Here we filter the customerAttributesGroups to be only the ones that have the passed key parameter as a key.
            var customerAttributesMappingByKey = from @group in customerAttributesGroups
                                                 where @group.Select(x => x.Attribute)
                                                             .Any(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) &&
                                                                  x.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                                                 select @group;

            return customerAttributesMappingByKey;
        }

        private IQueryable<Customer> GetCustomersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, int sinceId = 0)
        {
            var query = _customerRepository.TableNoTracking.Where(customer => !customer.Deleted && !customer.IsSystemAccount && customer.Active);

            query = query.Where(customer => !customer.CustomerRoles.Any(cr => (cr.Active) && (cr.SystemName == SystemCustomerRoleNames.Guests)));

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            query = query.OrderBy(customer => customer.Id);

            if (sinceId > 0)
            {
                query = query.Where(customer => customer.Id > sinceId);
            }

            return query;
        }
    }
}