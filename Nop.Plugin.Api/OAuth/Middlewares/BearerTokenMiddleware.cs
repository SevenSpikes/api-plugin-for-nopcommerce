using System.Threading.Tasks;
using Microsoft.Owin;

namespace Nop.Plugin.Api.Owin.Middleware
{
    /// <summary>
    /// Our own middleware that resets the current user set by the Forms authentication in case we have a Bearer token request
    /// This way we can have the OAuthBearerAuthenticationMiddleware to work properly
    /// </summary>
    public class BearerTokenMiddleware : OwinMiddleware
    {
        public BearerTokenMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            string authorization = context.Request.Headers.Get("Authorization");
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
            {
                context.Request.User = null;
            }

            return Next.Invoke(context);
        }
    }
}
