using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.DTO.TaxCategory;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Api.Controllers
{

    public class TaxCategoryController : BaseApiController
    {
        //private readonly IProductApiService _productApiService;
        //private readonly IProductService _productService;
        //private readonly IUrlRecordService _urlRecordService;
        //private readonly IManufacturerService _manufacturerService;
        private readonly IFactory<Product> _factory;
        //private readonly IProductTagService _productTagService;
        //private readonly IProductAttributeService _productAttributeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly IDTOHelper _dtoHelper;
        

        public TaxCategoryController(
                                  IJsonFieldsSerializer jsonFieldsSerializer,
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  IFactory<Product> factory,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  IProductTagService productTagService,
                                  ITaxCategoryService taxCategoryService,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _factory = factory;
            _dtoHelper = dtoHelper;
            _taxCategoryService = taxCategoryService;
        }

        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/taxcategories")]
        [ProducesResponseType(typeof(TaxCategoryRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetTaxCategories(TaxCategoriesParametersModel parameters)
        {
            var allTaxCategories = _taxCategoryService.GetAllTaxCategories();

            IList<TaxCategoryDto> taxCategoriesAsDtos = allTaxCategories.Select(taxCategory => _dtoHelper.PrepareTaxCategoryDTO(taxCategory)).ToList();

            var taxCategoriesRootObject = new TaxCategoryRootObjectDto()
            {
                TaxCategories = taxCategoriesAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(taxCategoriesRootObject, null);

            return new RawJsonActionResult(json);
        }

    }
}