using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class StoreController : BaseApiController
    {
        private IStoreContext _storeContext;

        public StoreController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IStoreContext storeContext)
            : base(jsonFieldsSerializer,
                  aclService,
                  customerService,
                  storeMappingService,
                  storeService,
                  discountService,
                  customerActivityService,
                  localizationService)
        {
            _storeContext = storeContext;
        }

        /// <summary>
        /// Retrieve category by spcified id
        /// </summary>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(StoresRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetCurrentStore(string fields = "")
        {
            Store store = _storeContext.CurrentStore;

            if (store == null)
            {
                return Error(HttpStatusCode.NotFound, "store", "store not found");
            }

            StoreDto storeDto = store.ToDto();

            var storesRootObject = new StoresRootObject();

            storesRootObject.Stores.Add(storeDto);

            var json = _jsonFieldsSerializer.Serialize(storesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
