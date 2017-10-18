using Nop.Plugin.Api.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Microsoft.AspNet.WebHooks;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using Nop.Plugin.Api.Helpers;
using System.Globalization;
using Nop.Core;
using Nop.Plugin.Api.Constants;
using Nop.Services.Media;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class WebHookRegistrationsController : BaseApiController
    {

        private IWebHookManager _manager;
        private IWebHookStore _store;
        private IWebHookUser _user;
        private readonly IAuthorizationHelper _authorizationHelper;
        private readonly IStoreContext _storeContext;
        private const string PRIVATE_FILTER_PREFIX = "MS_Private_";

        public WebHookRegistrationsController(IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IAuthorizationHelper authorizationHelper, 
            IStoreContext storeContext)
            : base(jsonFieldsSerializer,
                  aclService, customerService, 
                  storeMappingService, 
                  storeService, 
                  discountService, 
                  customerActivityService,
                  localizationService,
                  pictureService)
        {
            _authorizationHelper = authorizationHelper;
            _storeContext = storeContext;
        }

        /// <summary>
        /// Gets all registered WebHooks for a given user.
        /// </summary>
        /// <returns>A collection containing the registered <see cref="WebHook"/> instances for a given user.</returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<WebHook>))]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IEnumerable<WebHook>> GetAllWebHooks()
        {
            string userId = GetUserId();
            IEnumerable<WebHook> webHooks = await _store.GetAllWebHooksAsync(userId);
            RemovePrivateFilters(webHooks);
            return webHooks;
        }

        /// <summary>
        /// Looks up a registered WebHook with the given <paramref name="id"/> for a given user.
        /// </summary>
        /// <returns>The registered <see cref="WebHook"/> instance for a given user.</returns>
        [HttpGet]
        [ResponseType(typeof(WebHook))]
        [GetRequestsErrorInterceptorActionFilter]
        public async Task<IHttpActionResult> GetWebHookById(string id)
        {
            string userId = GetUserId();
            WebHook webHook = await _store.LookupWebHookAsync(userId, id);
            if (webHook != null)
            {
                RemovePrivateFilters(new[] { webHook });
                return Ok(webHook);
            }
            return NotFound();
        }

        /// <summary>
        /// Registers a new WebHook for a given user.
        /// </summary>
        /// <param name="webHook">The <see cref="WebHook"/> to create.</param>
        [HttpPost]
        [ResponseType(typeof(WebHook))]
        public async Task<IHttpActionResult> RegisterWebHook(WebHook webHook)
        {
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (webHook == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            await VerifyFilters(webHook);
            await VerifyWebHook(webHook);

            // In order to ensure that a web hook filter is not registered multiple times for the same uri
            // we remove the already registered filters from the current web hook.
            // If the same filters are registered multiple times with the same uri, the web hook event will be
            // sent for each registration.
            IEnumerable<WebHook> existingWebhooks = await GetAllWebHooks();
            IEnumerable<WebHook> existingWebhooksForTheSameUri = existingWebhooks.Where(wh => wh.WebHookUri == webHook.WebHookUri);

            foreach (var existingWebHook in existingWebhooksForTheSameUri)
            {
                webHook.Filters.ExceptWith(existingWebHook.Filters);

                if (!webHook.Filters.Any())
                {
                    string msg = _localizationService.GetResource("Api.WebHooks.CouldNotRegisterDuplicateWebhook");
                    HttpResponseMessage error = Request.CreateErrorResponse(HttpStatusCode.Conflict, msg);
                    return ResponseMessage(error);
                }
            }

            try
            {
                // Validate the provided WebHook ID (or force one to be created on server side)
                if (Request == null)
                {
                    throw new ArgumentNullException(nameof(Request));
                }

                // Ensure we have a normalized ID for the WebHook
                webHook.Id = null;

                // Add WebHook for this user.
                StoreResult result = await _store.InsertWebHookAsync(userId, webHook);

                if (result == StoreResult.Success)
                {
                    return CreatedAtRoute(WebHookNames.GetWebhookByIdAction, new { id = webHook.Id }, webHook);
                }
                return CreateHttpResult(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotRegisterWebhook"), ex.Message);
                Configuration.DependencyResolver.GetLogger().Error(msg, ex);
                HttpResponseMessage error = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg, ex);
                return ResponseMessage(error);
            }
        }

        /// <summary>
        /// Updates an existing WebHook registration.
        /// </summary>
        /// <param name="id">The WebHook ID.</param>
        /// <param name="webHook">The new <see cref="WebHook"/> to use.</param>
        [HttpPut]
        public async Task<IHttpActionResult> UpdateWebHook(string id, WebHook webHook)
        {
            if (webHook == null)
            {
                return BadRequest();
            }
            if (!string.Equals(id, webHook.Id, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest();
            }

            string userId = GetUserId();
            await VerifyFilters(webHook);
            await VerifyWebHook(webHook);

            try
            {
                StoreResult result = await _store.UpdateWebHookAsync(userId, webHook);
                return CreateHttpResult(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotUpdateWebhook"), ex.Message);
                Configuration.DependencyResolver.GetLogger().Error(msg, ex);
                HttpResponseMessage error = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg, ex);
                return ResponseMessage(error);
            }
        }

        /// <summary>
        /// Deletes an existing WebHook registration.
        /// </summary>
        /// <param name="id">The WebHook ID.</param>
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteWebHook(string id)
        {
            string userId = GetUserId();

            try
            {
                StoreResult result = await _store.DeleteWebHookAsync(userId, id);
                return CreateHttpResult(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotDeleteWebhook"), ex.Message);
                Configuration.DependencyResolver.GetLogger().Error(msg, ex);
                HttpResponseMessage error = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg, ex);
                return ResponseMessage(error);
            }
        }

        /// <summary>
        /// Deletes all existing WebHook registrations.
        /// </summary>
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteAllWebHooks()
        {
            string userId = GetUserId();

            try
            {
                await _store.DeleteAllWebHooksAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                string msg = string.Format(CultureInfo.InvariantCulture, _localizationService.GetResource("Api.WebHooks.CouldNotDeleteWebhooks"), ex.Message);
                Configuration.DependencyResolver.GetLogger().Error(msg, ex);
                HttpResponseMessage error = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, msg, ex);
                return ResponseMessage(error);
            }
        }

        /// <inheritdoc />
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            // The Microsoft.AspNet.WebHooks library registeres an extension method for the DependencyResolver.
            // Sadly we cannot access these properties by using out Autofac dependency injection.
            // In order to access them we have to resolve them through the Configuration.
            _manager = Configuration.DependencyResolver.GetManager();
            _store = Configuration.DependencyResolver.GetStore();
            _user = Configuration.DependencyResolver.GetUser();
        }

        /// <summary>
        /// Ensure that the provided <paramref name="webHook"/> only has registered filters.
        /// </summary>
        protected virtual async Task VerifyFilters(WebHook webHook)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

            // If there are no filters then add our wildcard filter.
            if (webHook.Filters.Count == 0)
            {
                webHook.Filters.Add(WildcardWebHookFilterProvider.Name);
                return;
            }

            IWebHookFilterManager filterManager = Configuration.DependencyResolver.GetFilterManager();
            IDictionary<string, WebHookFilter> filters = await filterManager.GetAllWebHookFiltersAsync();
            HashSet<string> normalizedFilters = new HashSet<string>();
            List<string> invalidFilters = new List<string>();
            foreach (string filter in webHook.Filters)
            {
                WebHookFilter hookFilter;
                if (filters.TryGetValue(filter, out hookFilter))
                {
                    normalizedFilters.Add(hookFilter.Name);
                }
                else
                {
                    invalidFilters.Add(filter);
                }
            }

            if (invalidFilters.Count > 0)
            {
                string invalidFiltersMsg = string.Join(", ", invalidFilters);
                string link = Url.Link(WebHookNames.FiltersGetAction, routeValues: null);
                string msg = string.Format(CultureInfo.CurrentCulture, _localizationService.GetResource("Api.WebHooks.InvalidFilters"), invalidFiltersMsg, link);
                Configuration.DependencyResolver.GetLogger().Info(msg);

                HttpResponseMessage response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, msg);
                throw new HttpResponseException(response);
            }
            else
            {
                webHook.Filters.Clear();
                foreach (string filter in normalizedFilters)
                {
                    webHook.Filters.Add(filter);
                }
            }
        }

        /// <summary>
        /// Removes all private filters from registered WebHooks.
        /// </summary>
        protected virtual void RemovePrivateFilters(IEnumerable<WebHook> webHooks)
        {
            if (webHooks == null)
            {
                throw new ArgumentNullException(nameof(webHooks));
            }

            foreach (WebHook webHook in webHooks)
            {
                var filters = webHook.Filters.Where(f => f.StartsWith(PRIVATE_FILTER_PREFIX, StringComparison.OrdinalIgnoreCase)).ToArray();
                foreach (string filter in filters)
                {
                    webHook.Filters.Remove(filter);
                }
            }
        }

        /// <summary>
        /// Ensures that the provided <paramref name="webHook"/> has a reachable Web Hook URI unless
        /// the WebHook URI has a <c>NoEcho</c> query parameter.
        /// </summary>
        private async Task VerifyWebHook(WebHook webHook)
        {
            if (webHook == null)
            {
                throw new ArgumentNullException(nameof(webHook));
            }

            // If no secret is provided then we create one here. This allows for scenarios
            // where the caller may use a secret directly embedded in the WebHook URI, or
            // has some other way of enforcing security.
            if (string.IsNullOrEmpty(webHook.Secret))
            {
                webHook.Secret = Guid.NewGuid().ToString("N");
            }

            try
            {
                await _manager.VerifyWebHookAsync(webHook);
            }
            catch (Exception ex)
            {
                HttpResponseMessage error = Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message, ex);
                throw new HttpResponseException(error);
            }
        }

        /// <summary>
        /// Gets the user ID for this request.
        /// </summary>
        private string GetUserId()
        {
            // If we are here the client is already authorized.
            // So there is a client ID and the client is active.
            var client = _authorizationHelper.GetCurrentClientFromClaims();

            var storeId = _storeContext.CurrentStore.Id;

            var webHookUser = client.ClientId + "-" + storeId;

            return webHookUser;
        }

        /// <summary>
        /// Creates an <see cref="IHttpActionResult"/> based on the provided <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The result to use when creating the <see cref="IHttpActionResult"/>.</param>
        /// <returns>An initialized <see cref="IHttpActionResult"/>.</returns>
        private IHttpActionResult CreateHttpResult(StoreResult result)
        {
            switch (result)
            {
                case StoreResult.Success:
                    return Ok();

                case StoreResult.Conflict:
                    return Conflict();

                case StoreResult.NotFound:
                    return NotFound();

                case StoreResult.OperationError:
                    return BadRequest();

                default:
                    return InternalServerError();
            }
        }
    }
}
