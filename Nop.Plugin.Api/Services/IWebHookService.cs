namespace Nop.Plugin.Api.Services
{
    using Microsoft.AspNetCore.WebHooks;

    public interface IWebHookService
    {
        IWebHookManager GetWebHookManager();
        IWebHookSender GetWebHookSender();
        IWebHookStore GetWebHookStore();
        IWebHookFilterManager GetWebHookFilterManager();
    }
}
