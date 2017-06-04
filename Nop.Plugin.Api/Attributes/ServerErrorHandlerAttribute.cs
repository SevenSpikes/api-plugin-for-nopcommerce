using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Http.Filters;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Logging;
using System.Web.Http;

namespace Nop.Plugin.Api.Attributes
{
    public class ServerErrorHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context != null && context.Exception != null)
            {
                string responseErrorMessage = string.Empty;

                var responseException = context.Exception as HttpResponseException;
                if (responseException != null)
                {
                    responseErrorMessage = responseException.Response.ToString();
                } else
                {
                    responseErrorMessage = context.Exception.Message;
                }

                var logger = EngineContext.Current.Resolve<ILogger>();
                logger.Error(responseErrorMessage, context.Exception);

                var jsonFieldsSerializer = EngineContext.Current.Resolve<IJsonFieldsSerializer>();

                var errors = new Dictionary<string, List<string>>()
                {
                    {
                        "server_error",
                        new List<string>() {"Please contact the store owner!"}
                    }
                };

                var errorsRootObject = new ErrorsRootObject()
                {
                    Errors = errors
                };

                var errorsJson = jsonFieldsSerializer.Serialize(errorsRootObject, null);

                var errorResult = new ErrorActionResult(errorsJson, HttpStatusCode.InternalServerError);

                context.Response = errorResult.ExecuteAsync(new CancellationToken()).Result;
            }
        }
    }
}