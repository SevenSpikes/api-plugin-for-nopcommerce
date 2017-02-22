using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.WebHooks;
using Nop.Plugin.Api.Constants;

namespace Nop.Plugin.Api.WebHooks
{
    public class FilterProvider : IWebHookFilterProvider
    {
        private readonly Collection<WebHookFilter> filters = new Collection<WebHookFilter>
    {
        new WebHookFilter { Name = WebHookNames.CustomerCreated, Description = "A customer has been registered."},
        new WebHookFilter { Name = WebHookNames.CustomerUpdated, Description = "A customer has been updated."},
        new WebHookFilter { Name = WebHookNames.CustomerDeleted, Description = "A customer has been deleted."},
        new WebHookFilter { Name = WebHookNames.ProductCreated, Description = "A product has been created."},
        new WebHookFilter { Name = WebHookNames.ProductUpdated, Description = "A product has been updated."},
        new WebHookFilter { Name = WebHookNames.ProductDeleted, Description = "A product has been deleted."},
        new WebHookFilter { Name = WebHookNames.CategoryCreated, Description = "A category has been created."},
        new WebHookFilter { Name = WebHookNames.CategoryUpdated, Description = "A category has been updated."},
        new WebHookFilter { Name = WebHookNames.CategoryDeleted, Description = "A category has been deleted."},
        new WebHookFilter { Name = WebHookNames.OrderCreated, Description = "An order has been created."},
        new WebHookFilter { Name = WebHookNames.OrderUpdated, Description = "An order has been updated."},
        new WebHookFilter { Name = WebHookNames.OrderDeleted, Description = "An order has been deleted."}
    };

        public Task<Collection<WebHookFilter>> GetFiltersAsync()
        {
            return Task.FromResult(this.filters);
        }
    }
}
