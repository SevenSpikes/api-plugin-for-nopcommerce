using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Api.Constants
{
    public static class WebHookNames
    {
        public const string FiltersGetAction = "FiltersGetAction";

        public const string GetWebhookByIdAction = "GetWebHookByIdAction";

        public const string CustomerCreated = "customer/created";
        public const string CustomerUpdated = "customer/updated";
        public const string CustomerDeleted = "customer/deleted";

        public const string ProductCreated = "product/created";
        public const string ProductUpdated = "product/updated";
        public const string ProductDeleted = "product/deleted";

        public const string CategoryCreated = "category/created";
        public const string CategoryUpdated = "category/updated";
        public const string CategoryDeleted = "category/deleted";

        public const string OrderCreated = "order/created";
        public const string OrderUpdated = "order/updated";
        public const string OrderDeleted = "order/deleted";
    }
}
