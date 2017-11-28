namespace Nop.Plugin.Api.Services
{
    using Microsoft.AspNet.WebHooks;

    public interface IWebHookService
    {
        IWebHookManager GetHookManager();
    }
}
