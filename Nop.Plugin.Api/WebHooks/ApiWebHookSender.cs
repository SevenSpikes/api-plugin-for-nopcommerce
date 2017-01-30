using System.Collections.Generic;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Nop.Plugin.Api.WebHooks
{
    public class ApiWebHookSender : DataflowWebHookSender
    {
        private const string WebHookIdKey = "WebHookId";

        public ApiWebHookSender(ILogger logger) : base(logger)
        {
        }

        /// <inheritdoc />
        protected override JObject CreateWebHookRequestBody(WebHookWorkItem workItem)
        {
            JObject data = base.CreateWebHookRequestBody(workItem);

            Dictionary<string, object> body = data.ToObject<Dictionary<string, object>>();

            body[WebHookIdKey] = workItem.WebHook.Id;

            return JObject.FromObject(body);
        }
    }
}
