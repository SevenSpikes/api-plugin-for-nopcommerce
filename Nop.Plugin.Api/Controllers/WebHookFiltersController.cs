using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.WebHooks;
using Nop.Plugin.Api.Attributes;
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
    public class WebHookFiltersController : BaseApiController
    {
        public WebHookFiltersController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService) :
            base(jsonFieldsSerializer, 
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

        [HttpGet]
        [ResponseType(typeof(IEnumerable<WebHookFilter>))]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IEnumerable<WebHookFilter>> GetWebHookFilters()
        {
            IWebHookFilterManager filterManager = Configuration.DependencyResolver.GetFilterManager();
            IDictionary<string, WebHookFilter> filters = await filterManager.GetAllWebHookFiltersAsync();
            return filters.Values;
        }
    }
}
