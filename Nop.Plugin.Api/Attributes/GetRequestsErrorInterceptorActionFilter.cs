using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Models;
using Nop.Plugin.Api.Serializers;

namespace Nop.Plugin.Api.Attributes
{
    using System.IO;
    using System.Text;

    public class GetRequestsErrorInterceptorActionFilter : ActionFilterAttribute
    {
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public GetRequestsErrorInterceptorActionFilter()
        {
            _jsonFieldsSerializer = EngineContext.Current.Resolve<IJsonFieldsSerializer>();
        }

        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.HttpContext.Response != null && (HttpStatusCode)actionExecutedContext.HttpContext.Response.StatusCode != HttpStatusCode.OK)
            {
                string responseBody = string.Empty;

                using (var streamReader = new StreamReader(actionExecutedContext.HttpContext.Response.Body))
                {
                    responseBody = streamReader.ReadToEnd();
                }

                // reset reader possition.
                actionExecutedContext.HttpContext.Response.Body.Position = 0;

                DefaultWeApiErrorsModel defaultWebApiErrorsModel = JsonConvert.DeserializeObject<DefaultWeApiErrorsModel>(responseBody);

                // If both are null this means that it is not the default web api error format, 
                // which means that it the error is formatted by our standard and we don't need to do anything.
                if (!string.IsNullOrEmpty(defaultWebApiErrorsModel.Message) &&
                    !string.IsNullOrEmpty(defaultWebApiErrorsModel.MessageDetail))
                {
                    Dictionary<string, List<string>> bindingError = new Dictionary<string, List<string>>()
                    {
                        {
                            "lookup_error",
                            new List<string>() {"not found"}
                        }
                    };

                    var errorsRootObject = new ErrorsRootObject()
                    {
                        Errors = bindingError
                    };

                    string errorJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

                    // TODO: test this.
                    actionExecutedContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    actionExecutedContext.HttpContext.Response.Body = new MemoryStream(Encoding.UTF8.GetBytes(errorJson));
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}