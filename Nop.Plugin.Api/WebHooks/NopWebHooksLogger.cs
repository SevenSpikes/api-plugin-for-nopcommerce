using Microsoft.AspNet.WebHooks.Diagnostics;
using Nop.Core.Infrastructure;
using System;
using System.Web.Http.Tracing;

namespace Nop.Plugin.Api.WebHooks
{
    public class NopWebHooksLogger : ILogger
    {
        public void Log(TraceLevel level, string message, Exception ex)
        {
            //TODO: Add a setting in the Administration to enable logging using the nopCommerce logger 
            //Nop.Services.Logging.ILogger logger = EngineContext.Current.Resolve<Nop.Services.Logging.ILogger>();
            //logger.InsertLog(Core.Domain.Logging.LogLevel.Information, message, ex?.ToString());
        }
    }
}
