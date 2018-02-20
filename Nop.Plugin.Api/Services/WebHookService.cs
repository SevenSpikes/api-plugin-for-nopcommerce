using Microsoft.AspNet.WebHooks;

using Microsoft.AspNet.WebHooks.Services;
using Nop.Plugin.Api.WebHooks;

namespace Nop.Plugin.Api.Services
{
    public class WebHookService : IWebHookService
    {
        private IWebHookManager _webHookManager;
        private IWebHookStore _webHookStore;

        public IWebHookManager GetHookManager()
        {
            if (_webHookManager == null || _webHookStore.GetType() != typeof(SqlWebHookStore))
            {
                Microsoft.AspNet.WebHooks.Diagnostics.ILogger logger = new Microsoft.AspNet.WebHooks.Diagnostics.TraceLogger();
                _webHookStore = CustomServices.GetStore();
                IWebHookSender sender = new ApiWebHookSender(logger);

                _webHookManager = new WebHookManager(_webHookStore, sender, logger);
            }

            return _webHookManager;
        }
    }
}
