using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;

namespace Nop.Plugin.Api.Services
{
    public interface IWebHookService
    {
        IWebHookManager GetHookManager();
    }
}
