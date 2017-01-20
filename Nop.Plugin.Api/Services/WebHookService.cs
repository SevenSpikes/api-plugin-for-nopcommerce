using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Microsoft.AspNet.WebHooks.Diagnostics;
using Microsoft.AspNet.WebHooks.Services;

namespace Nop.Plugin.Api.Services
{
    public class WebHookService : IWebHookService
    {
        private IWebHookManager _webHookManager;

        public IWebHookManager GetHookManager()
        {
            if (_webHookManager == null)
            {
                ILogger logger = new TraceLogger();
                IWebHookStore store = CustomServices.GetStore();
                IWebHookSender sender = CustomServices.GetSender(logger);

                _webHookManager = new WebHookManager(store, sender, logger);
            }

            return _webHookManager;
        }
    }
}
