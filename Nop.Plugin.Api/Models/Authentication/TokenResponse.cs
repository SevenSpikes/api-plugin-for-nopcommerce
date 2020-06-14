using Newtonsoft.Json;

namespace Nop.Plugin.Api.Models.Authentication
{
    public class TokenResponse
    {
        public TokenResponse(string errorDescription)
        {
            ErrorDescription = errorDescription;
        }

        public TokenResponse(string accessToken, long expiresInSeconds, string tokenType = "Bearer")
        {
            AccessToken = accessToken;
            ExpiresInSeconds = expiresInSeconds;
            TokenType = tokenType;
        }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = "Bearer";

        [JsonProperty("expires_in")]
        public long ExpiresInSeconds { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
