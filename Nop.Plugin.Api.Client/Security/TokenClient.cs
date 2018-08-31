using System;
using Nop.Plugin.Api.Client.Exceptions;
using RestSharp;

namespace Nop.Plugin.Api.Client.Security
{
    public class TokenClient
    {
        private readonly Uri _serverUrl;
        
        public TokenClient(Uri serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public Token GetToken(string clientId, string secret)
        {
            return GetTokenInternal(clientId, secret, GetAuthCode(clientId));
        }

        private string GetAuthCode(string clientId)
        {
            var restClient = new RestClient(_serverUrl.ToString());
            var request = new RestRequest("oauth/authorize") { Method = Method.GET };
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clientId);
            request.AddParameter("redirect_uri", $"{_serverUrl}token");
            request.AddParameter("response_type", "code");
            
            var response = restClient.Execute(request);
            if (response.ResponseUri != null)
            {
                var parameters = response.ResponseUri.Query.Replace("?", "").Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var p in parameters)
                {
                    var parts = p.Split('=');
                    if (parts.Length == 2)
                    {
                        if (parts[0].ToLower() == "code")
                        {
                            return parts[1];
                        }
                    }
                }
            }

            throw new NopApiClientException("Could not fetch auth code.");
        }

        private Token GetTokenInternal(string clientId, string secret, string code)
        {
            var restClient = new RestClient(_serverUrl.ToString());
            var request = new RestRequest("connect/token") { Method = Method.POST };
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", secret);
            request.AddParameter("code", code);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("redirect_uri", $"{_serverUrl}token");

            var tokenResponse = restClient.Execute<Token>(request);
            
            return tokenResponse.Data;
        }
    }
}
