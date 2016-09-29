using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Helpers;

namespace Nop.Plugin.Api.Attributes
{
    public class BearerTokenAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IAuthorizationHelper _authorizationHelper = EngineContext.Current.Resolve<IAuthorizationHelper>();

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            ApiSettings settings = EngineContext.Current.Resolve<ApiSettings>();

            // Swagger client does not support BearerToken authentication.
            // That is why we don't check for Bearer token authentication but check only 2 things:
            // 1. The store owner explicitly has allowed Swagger to make requests to the API
            // 2. Check if the request really comes from Swagger documentation page. Since Swagger documentation page is located on /swagger/ui/index we simply check that the Refferer contains "swagger"
            if (settings.AllowRequestsFromSwagger && actionContext.Request.Headers.Referrer != null && actionContext.Request.Headers.Referrer.ToString().Contains("swagger"))
            {
                return true;
            }

            // At this point the customer making the request is already authorised by the nopCommerce FormsAuthentication, so
            // we need to make sure several things before providing access to the requested resource:
            // 1. The request is a BearerToken request - since we support only BearerToken authorization
            // 2. The Api is enabled from the plugin settings
            // 3. The provided BearerToken is valid and the corresponding client exists in the database and is active.
            var authorization = actionContext.Request.Headers.Authorization;
            if (authorization == null || authorization.Scheme != "Bearer" || !settings.EnableApi || !_authorizationHelper.ClientExistsAndActive())
            {
                // don't authorize if any of the above is not true
                return false;
            }

            return base.IsAuthorized(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // By default nopCommerce uses Forms authentication so it redirects any unauthorised requests to the Login page.
            // We don't want any unauthorised requests to the Api endpoints to be redirected to the Login page.
            // To ensure this won't happen we need to suppress the forms authentication redirect.
            // This way the client will just receive a message like this "Unauthorized request" and won't see the Login page.
            HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(actionContext);
        }
    }
}