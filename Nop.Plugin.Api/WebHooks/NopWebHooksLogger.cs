using Microsoft.AspNet.WebHooks.Diagnostics;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Domain;
using System;
using System.Web.Http.Tracing;

namespace Nop.Plugin.Api.WebHooks
{
    public class NopWebHooksLogger : ILogger
    {
        public void Log(TraceLevel level, string message, Exception ex)
        {
            var settings = EngineContext.Current.Resolve<ApiSettings>();
            if (settings.EnableLogging)
            {
                Nop.Services.Logging.ILogger logger = EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>();
                logger.InsertLog(Core.Domain.Logging.LogLevel.Information, message, ex?.ToString());
            }
        }
    }
}
