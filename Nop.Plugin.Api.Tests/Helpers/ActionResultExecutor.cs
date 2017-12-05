namespace Nop.Plugin.Api.Tests.Helpers
{
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Rhino.Mocks;

    public static class ActionResultExecutor
    {
        public static HttpStatusCode ExecuteResult(IActionResult result)
        {
            var actionContext = MockRepository.GenerateStub<ActionContext>();
            actionContext.HttpContext = new DefaultHttpContext();

            result.ExecuteResultAsync(actionContext);
            var statusCode = actionContext.HttpContext.Response.StatusCode;

            return (HttpStatusCode)statusCode;
        }
    }
}