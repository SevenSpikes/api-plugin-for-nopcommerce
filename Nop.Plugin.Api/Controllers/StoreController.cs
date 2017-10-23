using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class StoreController : BaseApiController
    {
        private IStoreContext _storeContext;
        private readonly CurrencySettings _currencySettings;
        private readonly ICurrencyService _currencyService;
        private readonly ILanguageService _languageService;
        private readonly IDTOHelper _dtoHelper;

        public StoreController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreContext storeContext,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            ILanguageService languageService,
            IDTOHelper dtoHelper)
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
            _storeContext = storeContext;
            _currencySettings = currencySettings;
            _currencyService = currencyService;
            _languageService = languageService;
            _dtoHelper = dtoHelper;
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

            StoreDto storeDto = _dtoHelper.PrepareStoreDTO(store);

            var storesRootObject = new StoresRootObject();

            storesRootObject.Stores.Add(storeDto);

            var json = _jsonFieldsSerializer.Serialize(storesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve all stores
        /// </summary>
        /// <param name="fields">Fields from the store you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(StoresRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetAllStores(string fields = "")
        {
            IList<Store> allStores = _storeService.GetAllStores();
        
            IList<StoreDto> storesAsDto = new List<StoreDto>();

            foreach (var store in allStores)
            {
                var storeDto = _dtoHelper.PrepareStoreDTO(store);

                storesAsDto.Add(storeDto);
            }

            var storesRootObject = new StoresRootObject()
            {
                Stores = storesAsDto
            };

            var json = _jsonFieldsSerializer.Serialize(storesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
