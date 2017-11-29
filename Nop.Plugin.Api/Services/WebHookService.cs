using Nop.Plugin.Api.WebHooks;

namespace Nop.Plugin.Api.Services
{
    using Microsoft.AspNet.WebHooks;
    using Microsoft.AspNet.WebHooks.Diagnostics;
    using Microsoft.AspNet.WebHooks.Services;

    public class WebHookService : IWebHookService
    {
        private IWebHookManager _webHookManager;

        public IWebHookManager GetHookManager()
        {
            if (_webHookManager == null)
            {
                ILogger logger = new TraceLogger();
                IWebHookStore store = CustomServices.GetStore();
                IWebHookSender sender = new ApiWebHookSender(logger);

                _webHookManager = new WebHookManager(store, sender, logger);
            }

            return _webHookManager;
        }
    }
}
