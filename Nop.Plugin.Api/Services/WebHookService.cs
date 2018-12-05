using Nop.Plugin.Api.WebHooks;

namespace Nop.Plugin.Api.Services
{
    using Microsoft.AspNetCore.WebHooks;
    using Microsoft.Extensions.Logging;
    using Nop.Plugin.Api.Helpers;

    using System.Collections.Generic;


    public class WebHookService : IWebHookService
    {
        private IWebHookManager _webHookManager;
        private IWebHookSender _webHookSender;
        private IWebHookStore _webHookStore;
        private IWebHookFilterManager _webHookFilterManager;
        private ILogger<WebHookManager> _webHookManagerLogger;
        private ILogger<ApiWebHookSender> _apiWebHookSenderLogger;

        private readonly IConfigManagerHelper _configManagerHelper;

        public WebHookService(IConfigManagerHelper configManagerHelper, ILogger<WebHookManager> webHookManagerLogger,
            ILogger<ApiWebHookSender> apiWebHookSenderLogger)
        {
            _configManagerHelper = configManagerHelper;
            _webHookManagerLogger = webHookManagerLogger;
            _apiWebHookSenderLogger = apiWebHookSenderLogger;
        }

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
            if (_webHookManager == null)
            {
                _webHookManager = new WebHookManager(GetWebHookStore(), GetWebHookSender(), _webHookManagerLogger);
            }

            return _webHookManager;
        }

        public IWebHookSender GetWebHookSender()
        {
            if (_webHookSender == null)
            {
                _webHookSender = new ApiWebHookSender(_apiWebHookSenderLogger);
            }

            return _webHookSender;
        }

        public IWebHookStore GetWebHookStore()
        {
            if (_webHookStore == null)
            {
                _webHookStore = new NopWebHookStore();
            }

            return _webHookStore;
        }
    }
}
