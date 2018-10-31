using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Tax;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;

namespace Nop.Plugin.Api.Controllers
{
    using System.Net;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Mvc;
    using DTOs.Errors;
    using JSON.Serializers;

    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaxController : BaseApiController
    {
        private ITaxCategoryService _taxCategoryService;
        private readonly IDTOHelper _dtoHelper;

        public TaxController(IJsonFieldsSerializer jsonFieldsSerializer,
                IAclService aclService,
                ICustomerService customerService,
                IStoreMappingService storeMappingService,
                IStoreService storeService,
                IDiscountService discountService,
                ICustomerActivityService customerActivityService,
                ILocalizationService localizationService,
                IPictureService pictureService,
                ITaxCategoryService taxCategoryService,
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
            _taxCategoryService = taxCategoryService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Retrieve all taxes categories
        /// </summary>
        /// <param name="fields">Fields from the language you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/taxcategories")]
        [ProducesResponseType(typeof(TaxCategoriesRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetAllTaxCategories(string fields = "")
        {
            var allTaxCategories = _taxCategoryService.GetAllTaxCategories();

            IList<TaxCategoryDto> taxCategoriesAsDto = allTaxCategories.Select(taxCategory => _dtoHelper.PrepateTaxCategoryDto(taxCategory)).ToList();

            var taxCategoriesRootObject = new TaxCategoriesRootObject()
            {
                TaxCategories = taxCategoriesAsDto
            };

            var json = JsonFieldsSerializer.Serialize(taxCategoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}
