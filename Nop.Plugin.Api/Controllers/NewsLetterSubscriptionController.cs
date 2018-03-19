using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
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
    public class NewsLetterSubscriptionController : BaseApiController
    {
        private readonly INewsLetterSubscriptionApiService _newsLetterSubscriptionApiService;

        public NewsLetterSubscriptionController(IJsonFieldsSerializer jsonFieldsSerializer,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            IAclService aclService,
            ICustomerService customerService,
            INewsLetterSubscriptionApiService newsLetterSubscriptionApiService) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _newsLetterSubscriptionApiService = newsLetterSubscriptionApiService;
        }

        /// <summary>
        /// Receive a list of all NewsLetterSubscriptions
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(NewsLetterSubscriptionsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetNewsLetterSubscriptions(NewsLetterSubscriptionsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");
            }

            var newsLetterSubscriptions = _newsLetterSubscriptionApiService.GetNewsLetterSubscriptions(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.OnlyActive);

            List<NewsLetterSubscriptionDto> newsLetterSubscriptionsDtos = newsLetterSubscriptions.Select(nls => nls.ToDto()).ToList();

            var newsLetterSubscriptionsRootObject = new NewsLetterSubscriptionsRootObject()
            {
                NewsLetterSubscriptions = newsLetterSubscriptionsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(newsLetterSubscriptionsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
    }
}
