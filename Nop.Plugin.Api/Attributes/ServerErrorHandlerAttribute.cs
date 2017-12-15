using System.Collections.Generic;
using System.Net;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Services.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nop.Plugin.Api.Attributes
{
    public class ServerErrorHandlerAttribute : ExceptionFilterAttribute
    {
        // TODO: test this
        public override void OnException(ExceptionContext context)
        {
            if (context != null && context.Exception != null)
            {
                string responseErrorMessage = string.Empty;
                
                responseErrorMessage = context.Exception.Message;

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

                context.Result = errorResult;

                base.OnException(context);
            }
        }
    }
}