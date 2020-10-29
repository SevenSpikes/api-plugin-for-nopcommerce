using LinqToDB;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Data;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.DTO.Customers;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using static Nop.Plugin.Api.Infrastructure.Constants;

namespace Nop.Plugin.Api.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private const string FirstName = "firstname";
        private const string LastName = "lastname";
        private const string LanguageId = "languageid";
        private const string DateOfBirth = "dateofbirth";
        private const string Gender = "gender";
        private const string KeyGroup = "customer";

        private readonly IStoreContext _storeContext;
        private readonly ILanguageService _languageService;
        private readonly ICustomerService _customerService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<NewsLetterSubscription> _subscriptionRepository;
        private readonly IStaticCacheManager _cacheManager;

        public CustomerApiService(IRepository<Customer> customerRepository,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<NewsLetterSubscription> subscriptionRepository,
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingRepository,
            IRepository<CustomerRole> customerRoleRepository,
            IStoreContext storeContext,
            ILanguageService languageService,
            IStoreMappingService storeMappingService,
            IStaticCacheManager staticCacheManager,
            ICustomerService customerService)
        {
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _subscriptionRepository = subscriptionRepository;
            _customerCustomerRoleMappingRepository = customerCustomerRoleMappingRepository;
            _customerRoleRepository = customerRoleRepository;
            _storeContext = storeContext;
            _languageService = languageService;
            _storeMappingService = storeMappingService;
            _cacheManager = staticCacheManager;
            _customerService = customerService;
        }

        public virtual IQueryable<Customer> GetAllCustomers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           int affiliateId = 0, int vendorId = 0, int[] customerRoleIds = null,
           string email = null, string username = null, string firstName = null, string lastName = null,
           int dayOfBirth = 0, int monthOfBirth = 0,
           string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
           int pageIndex = 0, int pageSize = int.MaxValue,
           int sinceId = 0, 
           bool getOnlyTotalCount = false)
        {
            var query = _customerRepository.Table;
                       
                       
            if (createdFromUtc.HasValue)
                query = query.Where(c => createdFromUtc.Value <= c.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(c => createdToUtc.Value >= c.CreatedOnUtc);
            if (affiliateId > 0)
                query = query.Where(c => affiliateId == c.AffiliateId);
            if (vendorId > 0)
                query = query.Where(c => vendorId == c.VendorId);
            if (sinceId > 0)
                query = query.Where(c => c.Id > sinceId);

            query = query.Where(c => !c.Deleted);

            if (customerRoleIds != null && customerRoleIds.Length > 0)
            {
                query = query.Join(_customerCustomerRoleMappingRepository.Table, x => x.Id, y => y.CustomerId,
                        (x, y) => new { Customer = x, Mapping = y })
                    .Where(z => customerRoleIds.Contains(z.Mapping.CustomerRoleId))
                    .Select(z => z.Customer)
                    .Distinct();
            }

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(c => c.Email.Contains(email));
            if (!string.IsNullOrWhiteSpace(username))
                query = query.Where(c => c.Username.Contains(username));
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.FirstNameAttribute &&
                                z.Attribute.Value.Contains(firstName))
                    .Select(z => z.Customer);
            }

            if (!string.IsNullOrWhiteSpace(lastName))
            {
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.LastNameAttribute &&
                                z.Attribute.Value.Contains(lastName))
                    .Select(z => z.Customer);
            }

            //date of birth is stored as a string into database.
            //we also know that date of birth is stored in the following format YYYY-MM-DD (for example, 1983-02-18).
            //so let's search it as a string
            if (dayOfBirth > 0 && monthOfBirth > 0)
            {
                //both are specified
                var dateOfBirthStr = monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-" + dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                //z.Attribute.Value.Length - dateOfBirthStr.Length = 5
                //dateOfBirthStr.Length = 5
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Substring(5, 5) == dateOfBirthStr)
                    .Select(z => z.Customer);
            }
            else if (dayOfBirth > 0)
            {
                //only day is specified
                var dateOfBirthStr = dayOfBirth.ToString("00", CultureInfo.InvariantCulture);

                //z.Attribute.Value.Length - dateOfBirthStr.Length = 8
                //dateOfBirthStr.Length = 2
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Substring(8, 2) == dateOfBirthStr)
                    .Select(z => z.Customer);
            }
            else if (monthOfBirth > 0)
            {
                //only month is specified
                var dateOfBirthStr = "-" + monthOfBirth.ToString("00", CultureInfo.InvariantCulture) + "-";
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.DateOfBirthAttribute &&
                                z.Attribute.Value.Contains(dateOfBirthStr))
                    .Select(z => z.Customer);
            }
            //search by company
            if (!string.IsNullOrWhiteSpace(company))
            {
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.CompanyAttribute &&
                                z.Attribute.Value.Contains(company))
                    .Select(z => z.Customer);
            }
            //search by phone
            if (!string.IsNullOrWhiteSpace(phone))
            {
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.PhoneAttribute &&
                                z.Attribute.Value.Contains(phone))
                    .Select(z => z.Customer);
            }
            //search by zip
            if (!string.IsNullOrWhiteSpace(zipPostalCode))
            {
                query = query
                    .Join(_genericAttributeRepository.Table, x => x.Id, y => y.EntityId, (x, y) => new { Customer = x, Attribute = y })
                    .Where(z => z.Attribute.KeyGroup == nameof(Customer) &&
                                z.Attribute.Key == NopCustomerDefaults.ZipPostalCodeAttribute &&
                                z.Attribute.Value.Contains(zipPostalCode))
                    .Select(z => z.Customer);
            }

            //search by IpAddress
            if (!string.IsNullOrWhiteSpace(ipAddress) && CommonHelper.IsValidIpAddress(ipAddress))
            {
                query = query.Where(w => w.LastIpAddress == ipAddress);
            }

            query = query.OrderByDescending(c => c.CreatedOnUtc);

            return query;
        }

        public IList<CustomerDto> GetCustomersDtos(DateTime? createdAtMin = null, 
            DateTime? createdAtMax = null,
            int limit = Configurations.DefaultLimit,
            int page = Configurations.DefaultPageValue,
            int sinceId = Configurations.DefaultSinceId)
        {
            var result = new List<CustomerDto>();

            var roleIds = _customerRoleRepository.Table.Where(o => o.SystemName != NopCustomerDefaults.GuestsRoleName)
                .Select(o => o.Id)
                .ToArray();

            var customerAttributes = _genericAttributeRepository.Table.Where(o => o.KeyGroup == nameof(Customer));
          
            var customerWithAttributes = GetAllCustomers(createdAtMin, createdAtMax, customerRoleIds: roleIds, pageIndex: page - 1, pageSize: limit)
                .ToList()
                .GroupJoin(customerAttributes, outer => outer.Id, inner => inner.EntityId, (o, i) => new
                {
                    Customer = o,
                    Attributes = i
                });
           
            result.AddRange(customerWithAttributes.Select(o => CreateCustomerDto(o.Customer, o.Attributes)));

            SetNewsletterSubscriptionStatus(result);


            return result;
        }
        
        private CustomerDto CreateCustomerDto(Customer customer, IEnumerable<GenericAttribute> customerAttributes)
        {
            var customerDto = customer.ToDto();
            foreach (var attribute in customerAttributes)
            {
                
                if (attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                {
                    customerDto.FirstName = attribute.Value;
                }
                else if (attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    customerDto.LastName = attribute.Value;
                }
                else if (attribute.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase))
                {
                    customerDto.LanguageId = attribute.Value;
                }
                else if (attribute.Key.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
                {
                    customerDto.DateOfBirth = string.IsNullOrEmpty(attribute.Value) ? (DateTime?)null : DateTime.Parse(attribute.Value);
                }
                else if (attribute.Key.Equals(Gender, StringComparison.InvariantCultureIgnoreCase))
                {
                    customerDto.Gender = attribute.Value;
                }
            }
            return customerDto;
        }

        public int GetCustomersCount()
        {
            return _customerRepository.Table.Count(customer => !customer.Deleted
                                      && (customer.RegisteredInStoreId == 0 || customer.RegisteredInStoreId == _storeContext.CurrentStore.Id));
        }

        // Need to work with dto object so we can map the first and last name from generic attributes table.
        public IList<CustomerDto> Search(string queryParams = "", string order = Configurations.DefaultOrder,
            int page = Configurations.DefaultPageValue, int limit = Configurations.DefaultLimit)
        {
            IList<CustomerDto> result = new List<CustomerDto>();

            var searchParams = EnsureSearchQueryIsValid(queryParams, ParseSearchQuery);

            if (searchParams != null)
            {
                var query = _customerRepository.Table.Where(customer => !customer.Deleted);

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

                //result = HandleCustomerGenericAttributes(searchParams, query, limit, page, order);
            }

            return result;
        }

        public Customer GetCustomerEntityById(int id)
        {
            var customer = _customerRepository.Table.FirstOrDefault(c => c.Id == id && !c.Deleted);

            return customer;
        }

        public CustomerDto GetCustomerById(int id, bool showDeleted = false)
        {
            if (id == 0)
                return null;

            // Here we expect to get two records, one for the first name and one for the last name.
            var customerAttributeMappings = (from customer in _customerRepository.Table //NoTracking
                                             join attribute in _genericAttributeRepository.Table//NoTracking
                                                                                                on customer.Id equals attribute.EntityId
                                             where customer.Id == id &&
                                                   attribute.KeyGroup == "Customer"
                                             select new CustomerAttributeMappingDto
                                             {
                                                 Attribute = attribute,
                                                 Customer = customer
                                             }).ToList();

            CustomerDto customerDto = null;

            // This is in case we have first and last names set for the customer.
            if (customerAttributeMappings.Count > 0)
            {
                var customer = customerAttributeMappings.First().Customer;
                // The customer object is the same in all mappings.
                customerDto = customer.ToDto();

                var customerRoles = _customerService.GetCustomerRoles(customer);
                foreach (var role in customerRoles)
                    customerDto.RoleIds.Add(role.Id);

                var defaultStoreLanguageId = GetDefaultStoreLangaugeId();

                // If there is no Language Id generic attribute create one with the default language id.
                if (!customerAttributeMappings.Any(cam => cam?.Attribute != null && cam.Attribute.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var languageId = new GenericAttribute
                    {
                        Key = LanguageId,
                        Value = defaultStoreLanguageId.ToString()
                    };

                    var customerAttributeMappingDto = new CustomerAttributeMappingDto
                    {
                        Customer = customer,
                        Attribute = languageId
                    };

                    customerAttributeMappings.Add(customerAttributeMappingDto);
                }

                foreach (var mapping in customerAttributeMappings)
                {
                    if (!showDeleted && mapping.Customer.Deleted)
                    {
                        continue;
                    }

                    if (mapping.Attribute != null)
                    {
                        if (mapping.Attribute.Key.Equals(FirstName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.FirstName = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(LastName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.LastName = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(LanguageId, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.LanguageId = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(DateOfBirth, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.DateOfBirth = string.IsNullOrEmpty(mapping.Attribute.Value) ? (DateTime?)null : DateTime.Parse(mapping.Attribute.Value);
                        }
                        else if (mapping.Attribute.Key.Equals(Gender, StringComparison.InvariantCultureIgnoreCase))
                        {
                            customerDto.Gender = mapping.Attribute.Value;
                        }
                    }
                }
            }
            else
            {
                // This is when we do not have first and last name set.
                var currentCustomer = _customerRepository.Table.FirstOrDefault(customer => customer.Id == id);

                if (currentCustomer != null)
                {
                    if (showDeleted || !currentCustomer.Deleted)
                    {
                        customerDto = currentCustomer.ToDto();
                    }
                }
            }

            SetNewsletterSubscriptionStatus(customerDto);

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

            var splitPattern = @"(\w+):";

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != String.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (var i = 0; i < fieldValueList.Count; i += 2)
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

        private int GetDefaultStoreLangaugeId()
        {
            // Get the default language id for the current store.
            var defaultLanguageId = _storeContext.CurrentStore.DefaultLanguageId;

            if (defaultLanguageId == 0)
            {
                var allLanguages = _languageService.GetAllLanguages();

                var storeLanguages = allLanguages.Where(l =>
                    _storeMappingService.Authorize(l, _storeContext.CurrentStore.Id)).ToList();

                // If there is no language mapped to the current store, get all of the languages,
                // and use the one with the first display order. This is a default nopCommerce workflow.
                if (storeLanguages.Count == 0)
                {
                    storeLanguages = allLanguages.ToList();
                }

                var defaultLanguage = storeLanguages.OrderBy(l => l.DisplayOrder).First();

                defaultLanguageId = defaultLanguage.Id;
            }

            return defaultLanguageId;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        private void SetNewsletterSubscriptionStatus(IEnumerable<CustomerDto> customerDtos)
        {
            if (customerDtos == null)
            {
                return;
            }

            var allNewsletterCustomerEmail = GetAllNewsletterCustomersEmails();

            foreach (var customerDto in customerDtos)
            {
                SetNewsletterSubscriptionStatus(customerDto, allNewsletterCustomerEmail);
            }
        }

        private void SetNewsletterSubscriptionStatus(BaseCustomerDto customerDto, IEnumerable<String> allNewsletterCustomerEmail = null)
        {
            if (customerDto == null || String.IsNullOrEmpty(customerDto.Email))
            {
                return;
            }

            if (allNewsletterCustomerEmail == null)
            {
                allNewsletterCustomerEmail = GetAllNewsletterCustomersEmails();
            }

            if (allNewsletterCustomerEmail != null && allNewsletterCustomerEmail.Contains(customerDto.Email.ToLowerInvariant()))
            {
                customerDto.SubscribedToNewsletter = true;
            }
        }

        private IEnumerable<String> GetAllNewsletterCustomersEmails()
        {
            return _cacheManager.Get(Configurations.NEWSLETTER_SUBSCRIBERS_KEY, () =>
            {
                IEnumerable<String> subscriberEmails = (from nls in _subscriptionRepository.Table
                                                        where nls.StoreId == _storeContext.CurrentStore.Id
                                                              && nls.Active
                                                        select nls.Email).ToList();


                subscriberEmails = subscriberEmails.Where(e => !String.IsNullOrEmpty(e)).Select(e => e.ToLowerInvariant());

                return subscriberEmails.Where(e => !String.IsNullOrEmpty(e)).Select(e => e.ToLowerInvariant());
            });
        }
    }
}

