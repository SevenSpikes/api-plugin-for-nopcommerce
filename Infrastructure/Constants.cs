using Nop.Core.Caching;

namespace Nop.Plugin.Api.Infrastructure
{
    public static class Constants
    {
        public static class Roles
        {
            public const string ApiRoleSystemName = "ApiUserRole";

            public const string ApiRoleName = "Api Users";
        }

        public static class ViewNames
        {
            public const string AdminLayout = "_AdminLayout";
            public const string AdminApiSettings = "~/Plugins/Nop.Plugin.Api/Views/Settings.cshtml";
            public const string AdminApiClientsCreateOrUpdate = "~/Plugins/Nop.Plugin.Api/Views/Clients/CreateOrUpdate.cshtml";
            public const string AdminApiClientsSettings = "~/Plugins/Nop.Plugin.Api/Views/Clients/ClientSettings.cshtml";
            public const string AdminApiClientsList = "~/Plugins/Nop.Plugin.Api/Views/Clients/List.cshtml";
            public const string AdminApiClientsCreate = "~/Plugins/Nop.Plugin.Api/Views/Clients/Create.cshtml";
            public const string AdminApiClientsEdit = "~/Plugins/Nop.Plugin.Api/Views/Clients/Edit.cshtml";
        }

        public static class Configurations
        {
            public const int DefaultAccessTokenExpirationInDays = 3650; // 10 years

            
            public const int DefaultLimit = 50;
            public const int DefaultPageValue = 1;
            public const int DefaultSinceId = 0;
            public const int DefaultCustomerId = 0;
            public const string DefaultOrder = "Id";
            public const int MaxLimit = 250;

            public const int MinLimit = 1;


       
            public const string PublishedStatus = "published";
            public const string UnpublishedStatus = "unpublished";
            public const string AnyStatus = "any";
        

            public const string FixedRateSettingsKey = "Tax.TaxProvider.FixedOrByCountryStateZip.TaxCategoryId{0}";

            //public const string PublishedStatus = "published";
            //public const string UnpublishedStatus = "unpublished";
            //public const string AnyStatus = "any";
            public static CacheKey JsonTypeMapsPattern => new CacheKey("json.maps");

            public static CacheKey NEWSLETTER_SUBSCRIBERS_KEY = new CacheKey("Nop.api.newslettersubscribers");
        }
    }
}
