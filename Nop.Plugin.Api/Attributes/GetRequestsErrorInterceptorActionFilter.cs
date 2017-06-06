using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Models;
using Nop.Plugin.Api.Serializers;

namespace Nop.Plugin.Api.Attributes
{
    public class GetRequestsErrorInterceptorActionFilter : ActionFilterAttribute
    {
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public GetRequestsErrorInterceptorActionFilter()
        {
            _jsonFieldsSerializer = EngineContext.Current.Resolve<IJsonFieldsSerializer>();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Response != null && actionExecutedContext.Response.StatusCode != HttpStatusCode.OK)
            {
                var content = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;

                DefaultWeApiErrorsModel defaultWebApiErrorsModel = JsonConvert.DeserializeObject<DefaultWeApiErrorsModel>(content);

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

                    var errorJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(
                        HttpStatusCode.BadRequest, errorJson);
                }
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}