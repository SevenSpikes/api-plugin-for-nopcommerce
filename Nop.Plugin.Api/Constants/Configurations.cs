namespace Nop.Plugin.Api.Constants
{
    public class Configurations
    {
        // time is in minutes (30 days = 43200 minutes)
        // It is recommended to keep your access token expiration time to 1 hour and to use the refresh token to obtain a new one after expiration.
        // Refresh token lifespan should be max one month. 
        // Please, edit the configuration bellow!!!
        public const int DefaultAccessTokenExpiration = int.MaxValue;
        public const int DefaultRefreshTokenExpiration = int.MaxValue;
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
        public const string JsonTypeMapsPattern = "json.maps";
    }
}