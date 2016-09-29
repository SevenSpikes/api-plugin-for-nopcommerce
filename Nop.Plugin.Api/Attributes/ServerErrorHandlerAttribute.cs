using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Http.Filters;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.Serializers;
using Nop.Services.Logging;

namespace Nop.Plugin.Api.Attributes
{
    public class ServerErrorHandlerAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var jsonFieldsSerializer = EngineContext.Current.Resolve<IJsonFieldsSerializer>();

                var errors = new Dictionary<string, List<string>>()
                {
                    {
                        "server_error",
                        new List<string>() {"Please contact the store owner!"}
                    }
                };

                var logger = EngineContext.Current.Resolve<ILogger>();
                
                logger.Error(context.Exception.Message, context.Exception);

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