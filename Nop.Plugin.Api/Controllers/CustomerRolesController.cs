using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.CustomerRoles;
using Nop.Plugin.Api.DTOs.Languages;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CustomerRolesController : BaseApiController
    {
        public CustomerRolesController(
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService,
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService,
            IPictureService pictureService) 
            : base(jsonFieldsSerializer, 
                  aclService, 
                  customerService, 
                  storeMappingService, 
                  storeService, 
                  discountService,
                  customerActivityService,
                  localizationService, 
                  pictureService)
        {
        }

        /// <summary>
        /// Retrieve all customer roles
        /// </summary>
        /// <param name="fields">Fields from the customer role you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomerRolesRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetAllCustomerRoles(string fields = "")
        {
            IList<CustomerRole> allCustomerRoles = _customerService.GetAllCustomerRoles();

            IList<CustomerRoleDto> customerRolesAsDto = allCustomerRoles.Select(role => role.ToDto()).ToList();

            var customerRolesRootObject = new CustomerRolesRootObject()
            {
                CustomerRoles = customerRolesAsDto
            };

            var json = _jsonFieldsSerializer.Serialize(customerRolesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
