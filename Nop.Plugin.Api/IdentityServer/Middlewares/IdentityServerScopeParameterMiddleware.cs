namespace Nop.Plugin.Api.IdentityServer.Middlewares
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.Primitives;
    using Nop.Plugin.Api.IdentityServer.Extensions;

    public class IdentityServerScopeParameterMiddleware
    {
        private readonly RequestDelegate _next;

        public IdentityServerScopeParameterMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Equals("/connect/authorize", StringComparison.InvariantCultureIgnoreCase) ||
                context.Request.Path.Value.Equals("/oauth/authorize", StringComparison.InvariantCultureIgnoreCase))
            {
                if (!context.Request.Query.ContainsKey("scope"))
                {
                    var queryValues = new Dictionary<string, StringValues>();

                    foreach (var item in context.Request.Query)
                    {
                        queryValues.Add(item.Key, item.Value);
                    }

                    queryValues.Add("scope", "nop_api offline_access");

                    var newQueryCollection = new QueryCollection(queryValues);

                    context.Request.Query = newQueryCollection;
                }
            }

            await _next(context);
        }
    }
}