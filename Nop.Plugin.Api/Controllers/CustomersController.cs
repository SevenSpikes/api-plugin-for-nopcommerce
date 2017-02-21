using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CustomersController : BaseApiController
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly ICustomerRolesHelper _customerRolesHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IEncryptionService _encryptionService;
        private readonly ICountryService _countryService;
        private readonly IMappingHelper _mappingHelper;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IFactory<Customer> _factory;

        // We resolve the customer settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private CustomerSettings _customerSettings;

        private CustomerSettings CustomerSettings
        {
            get
            {
                if (_customerSettings == null)
                {
                    _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
                }

                return _customerSettings;
            }
        }

        public CustomersController(
            ICustomerApiService customerApiService, 
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerRolesHelper customerRolesHelper,
            IGenericAttributeService genericAttributeService,
            IEncryptionService encryptionService,
            IFactory<Customer> factory, 
            ICountryService countryService, 
            IMappingHelper mappingHelper, 
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService) : 
            base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService,pictureService)
        {
            _customerApiService = customerApiService;
            _factory = factory;
            _countryService = countryService;
            _mappingHelper = mappingHelper;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _customerRolesHelper = customerRolesHelper;
        }

        /// <summary>
        /// Retrieve all customers of a shop
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetCustomers(CustomersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            IList<CustomerDto> allCustomers = _customerApiService.GetCustomersDtos(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId);

            var customersRootObject = new CustomersRootObject()
            {
                Customers = allCustomers
            };

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve customer by spcified id
        /// </summary>
        /// <param name="id">Id of the customer</param>
        /// <param name="fields">Fields from the customer you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetCustomerById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            CustomerDto customer = _customerApiService.GetCustomerById(id);

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }
            
            var customersRootObject = new CustomersRootObject();
            customersRootObject.Customers.Add(customer);

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        /// Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomersCountRootObject))]
        public IHttpActionResult GetCustomersCount()
        {
            var allCustomersCount = _customerApiService.GetCustomersCount();

            var customersCountRootObject = new CustomersCountRootObject()
            {
                Count = allCustomersCount
            };

            return Ok(customersCountRootObject);
        }

        /// <summary>
        /// Search for customers matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        public IHttpActionResult Search(CustomersSearchParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit" ,"Invalid limit parameter");
            }

            if (parameters.Page <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }
            
            IList<CustomerDto> customersDto = _customerApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

            var customersRootObject = new CustomersRootObject()
            {
                Customers = customersDto
            };

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult CreateCustomer([ModelBinder(typeof(JsonModelBinder<CustomerDto>))] Delta<CustomerDto> customerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the customerDelta object won't be null for sure so we don't need to check for this.

            // Inserting the new customer
            Customer newCustomer = _factory.Initialize();
            customerDelta.Merge(newCustomer);

            foreach (var address in customerDelta.Dto.CustomerAddresses)
            {
                newCustomer.Addresses.Add(address.ToEntity());
            }
            
            _customerService.InsertCustomer(newCustomer);

            InsertFirstAndLastNameGenericAttributes(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, newCustomer);

            //password
            if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password))
            {
                AddPassword(customerDelta.Dto.Password, newCustomer);
            }
            
            // We need to insert the entity first so we can have its id in order to map it to anything.
            // TODO: Localization
            // TODO: move this before inserting the customer.
            if (customerDelta.Dto.RoleIds.Count > 0)
            {
                AddValidRoles(customerDelta, newCustomer);

                _customerService.UpdateCustomer(newCustomer);
            }

            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            CustomerDto newCustomerDto = newCustomer.ToDto();

            // This is needed because the entity framework won't populate the navigation properties automatically
            // and the country will be left null. So we do it by hand here.
            PopulateAddressCountryNames(newCustomerDto);

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            newCustomerDto.FirstName = customerDelta.Dto.FirstName;
            newCustomerDto.LastName = customerDelta.Dto.LastName;

            newCustomerDto.RoleIds = newCustomer.CustomerRoles.Select(x => x.Id).ToList();

            //activity log
            _customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), newCustomer.Id);

            var customersRootObject = new CustomersRootObject();

            customersRootObject.Customers.Add(newCustomerDto);

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
        
        [HttpPut]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult UpdateCustomer([ModelBinder(typeof(JsonModelBinder<CustomerDto>))] Delta<CustomerDto> customerDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //If the validation has passed the customerDelta object won't be null for sure so we don't need to check for this.
            
            // Updateting the customer
            Customer currentCustomer = _customerApiService.GetCustomerEntityById(int.Parse(customerDelta.Dto.Id));

            if (currentCustomer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            customerDelta.Merge(currentCustomer);

            if (customerDelta.Dto.RoleIds.Count > 0)
            {
                // Remove all roles
                while (currentCustomer.CustomerRoles.Count > 0)
                {
                    currentCustomer.CustomerRoles.Remove(currentCustomer.CustomerRoles.First());
                }

                AddValidRoles(customerDelta, currentCustomer);
            }

            if (customerDelta.Dto.CustomerAddresses.Count > 0)
            {
                var currentCustomerAddresses = currentCustomer.Addresses.ToDictionary(address => address.Id, address => address);

                foreach (var passedAddress in customerDelta.Dto.CustomerAddresses)
                {
                    int passedAddressId = int.Parse(passedAddress.Id);
                    Address addressEntity = passedAddress.ToEntity();

                    if (currentCustomerAddresses.ContainsKey(passedAddressId))
                    {
                        _mappingHelper.Merge(passedAddress, currentCustomerAddresses[passedAddressId]);
                    }
                    else
                    {
                        currentCustomer.Addresses.Add(addressEntity);
                    }
                }
            }

            _customerService.UpdateCustomer(currentCustomer);

            InsertFirstAndLastNameGenericAttributes(customerDelta.Dto.FirstName, customerDelta.Dto.LastName, currentCustomer);

            //password
            if (!string.IsNullOrWhiteSpace(customerDelta.Dto.Password))
            {
                AddPassword(customerDelta.Dto.Password, currentCustomer);
            }
            
            // TODO: Localization
           
            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            CustomerDto updatedCustomer = currentCustomer.ToDto();

            // This is needed because the entity framework won't populate the navigation properties automatically
            // and the country name will be left empty because the mapping depends on the navigation property
            // so we do it by hand here.
            PopulateAddressCountryNames(updatedCustomer);

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            var firstNameGenericAttribute = _genericAttributeService.GetAttributesForEntity(currentCustomer.Id, typeof(Customer).Name)
                .FirstOrDefault(x => x.Key == "FirstName");

            if (firstNameGenericAttribute != null)
            {
                updatedCustomer.FirstName = firstNameGenericAttribute.Value;
            }

            var lastNameGenericAttribute = _genericAttributeService.GetAttributesForEntity(currentCustomer.Id, typeof(Customer).Name)
                .FirstOrDefault(x => x.Key == "LastName");

            if (lastNameGenericAttribute != null)
            {
                updatedCustomer.LastName = lastNameGenericAttribute.Value;
            }

            updatedCustomer.RoleIds = currentCustomer.CustomerRoles.Select(x => x.Id).ToList();

            //activity log
            _customerActivityService.InsertActivity("UpdateCustomer", _localizationService.GetResource("ActivityLog.UpdateCustomer"), currentCustomer.Id);

            var customersRootObject = new CustomersRootObject();

            customersRootObject.Customers.Add(updatedCustomer);

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteCustomer(int id)
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            Customer customer = _customerApiService.GetCustomerEntityById(id);

            if (customer == null)
            {
                return Error(HttpStatusCode.NotFound, "customer", "not found");
            }

            _customerService.DeleteCustomer(customer);

            //remove newsletter subscription (if exists)
            foreach (var store in _storeService.GetAllStores())
            {
                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                if (subscription != null)
                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
            }

            //activity log
            _customerActivityService.InsertActivity("DeleteCustomer", _localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id);
            
            return new RawJsonActionResult("{}");
        }

        private void InsertFirstAndLastNameGenericAttributes(string firstName, string lastName, Customer newCustomer)
        {
            // we assume that if the first name is not sent then it will be null and in this case we don't want to update it
            if (firstName != null)
            {
                _genericAttributeService.SaveAttribute(newCustomer, SystemCustomerAttributeNames.FirstName, firstName);
            }

            if (lastName != null)
            {
                _genericAttributeService.SaveAttribute(newCustomer, SystemCustomerAttributeNames.LastName, lastName);
            }
        }

        private void AddValidRoles(Delta<CustomerDto> customerDelta, Customer currentCustomer)
        {
            IList<CustomerRole> validCustomerRoles =
                _customerRolesHelper.GetValidCustomerRoles(customerDelta.Dto.RoleIds).ToList();

            // Add all newly passed roles
            foreach (var role in validCustomerRoles)
            {
                currentCustomer.CustomerRoles.Add(role);
            }
        }

        private void PopulateAddressCountryNames(CustomerDto newCustomerDto)
        {
            foreach (var address in newCustomerDto.CustomerAddresses)
            {
                SetCountryName(address);
            }

            if (newCustomerDto.BillingAddress != null)
            {
                SetCountryName(newCustomerDto.BillingAddress);
            }

            if (newCustomerDto.ShippingAddress != null)
            {
                SetCountryName(newCustomerDto.ShippingAddress);
            }
        }

        private void SetCountryName(AddressDto address)
        {
            if (string.IsNullOrEmpty(address.CountryName) && address.CountryId.HasValue)
            {
                Country country = _countryService.GetCountryById(address.CountryId.Value);
                address.CountryName = country.Name;
            }
        }

        private void AddPassword(string newPassword, Customer customer)
        {
            // TODO: call this method before inserting the customer.
            switch (CustomerSettings.DefaultPasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        customer.Password = newPassword;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        customer.Password = _encryptionService.EncryptText(newPassword);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        string saltKey = _encryptionService.CreateSaltKey(5);
                        customer.PasswordSalt = saltKey;
                        customer.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey, CustomerSettings.HashedPasswordFormat);
                    }
                    break;
            }
            
            customer.PasswordFormat = CustomerSettings.DefaultPasswordFormat;

            // TODO: remove this.
            _customerService.UpdateCustomer(customer);
        }
    }
}