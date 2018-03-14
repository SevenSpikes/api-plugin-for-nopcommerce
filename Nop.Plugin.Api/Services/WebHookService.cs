using Nop.Plugin.Api.WebHooks;

namespace Nop.Plugin.Api.Services
{
    using Microsoft.AspNet.WebHooks;
    using Microsoft.AspNet.WebHooks.Diagnostics;
    using Microsoft.AspNet.WebHooks.Services;
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Tracing;

    public class WebHookService : IWebHookService
    {
        private IWebHookManager _webHookManager;
        private IWebHookSender _webHookSender;
        private IWebHookStore _webHookStore;
        private IWebHookFilterManager _webHookFilterManager;

        private ILogger _logger = new NopWebHooksLogger();

        public IWebHookFilterManager GetWebHookFilterManager()
        {
            if (_webHookFilterManager == null)
            {
                var filterProviders = new List<IWebHookFilterProvider>();
                filterProviders.Add(new FilterProvider());
                _webHookFilterManager = new WebHookFilterManager(filterProviders);
            }

            return _webHookFilterManager;
        }

        public IWebHookManager GetWebHookManager()
        {
            if (_webHookManager == null || _webHookStore.GetType() != typeof(SqlWebHookStore))
            {
                _webHookManager = new WebHookManager(GetWebHookStore(), GetWebHookSender(), _logger);
            }

            return _webHookManager;
        }

        public IWebHookSender GetWebHookSender()
        {
            if (_webHookSender == null)
            {
                _webHookSender = new ApiWebHookSender(_logger);
            }

            return _webHookSender;
        }

        public IWebHookStore GetWebHookStore()
        {
            if (_webHookStore == null || _webHookStore.GetType() != typeof(SqlWebHookStore))
            {
                _webHookStore = CustomServices.GetStore();
            }

            return _webHookStore;
        }
    }
}
