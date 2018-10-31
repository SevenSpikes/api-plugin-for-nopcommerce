using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Manufacturers;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Models.ManufacturersParameters;
using Nop.Services.Catalog;
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
    using Nop.Plugin.Api.Services;

    [ApiAuthorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ManufacturerController : BaseApiController
    {
        private IManufacturerApiService _manufacturerApiService;
        private readonly IDTOHelper _dtoHelper;

        public ManufacturerController(IJsonFieldsSerializer jsonFieldsSerializer,
                IAclService aclService,
                ICustomerService customerService,
                IStoreMappingService storeMappingService,
                IStoreService storeService,
                IDiscountService discountService,
                ICustomerActivityService customerActivityService,
                ILocalizationService localizationService,
                IPictureService pictureService,
                IManufacturerApiService manufacturerApiService,
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
            _manufacturerApiService = manufacturerApiService;
            _dtoHelper = dtoHelper;
        }

        /// <summary>
        /// Retrieve all manufacturers
        /// </summary>
        /// <param name="fields">Fields from the language you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturers(ManufacturersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            var manufacturers = _manufacturerApiService.GetManufacturers(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId);

            IList<ManufacturerDto> manufacturersAsDtos = manufacturers.Select(x => _dtoHelper.PrepareManufacturerDTO(x)).ToList();

            var manufacturersRootObject = new ManufacturersRootObject()
            {
                Manufacturers = manufacturersAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve manufacturer by specified id
        /// </summary>
        ///   /// <param name="id">Id of the manufacturer</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers/{id:int}")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetManufacturerById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var manufacturer = _manufacturerApiService.GetManufacturerById(id);

            if (manufacturer == null)
            {
                return Error(HttpStatusCode.NotFound, "manufacturer", "not found");
            }

            var manufacturersRootObject = new ManufacturersRootObject();

            var manufacturerDto = _dtoHelper.PrepareManufacturerDTO(manufacturer);
            manufacturersRootObject.Manufacturers.Add(manufacturerDto);

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Search for manufacturers matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/manufacturers/search")]
        [ProducesResponseType(typeof(ManufacturersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public IActionResult Search(ManufacturersSearchParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            var manufacturers = _manufacturerApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

            IList<ManufacturerDto> manufacturersAsDtos = manufacturers.Select(x => _dtoHelper.PrepareManufacturerDTO(x)).ToList();

            var manufacturersRootObject = new ManufacturersRootObject()
            {
                Manufacturers = manufacturersAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(manufacturersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
    }
}
