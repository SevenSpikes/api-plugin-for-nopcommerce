﻿using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Stores;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Stores;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using System.Collections.Generic;
using System.Net;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class StoreController : BaseApiController
    {
        private readonly IStoreContext _storeContext;
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
        [Route("/api/current_store")]
        [ProducesResponseType(typeof(StoresRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetCurrentStore(string fields = "")
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
        [Route("/api/stores")]
        [ProducesResponseType(typeof(StoresRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllStores(string fields = "")
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
