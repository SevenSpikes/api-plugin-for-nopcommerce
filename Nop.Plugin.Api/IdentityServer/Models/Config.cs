namespace Nop.Plugin.Api.IdentityServer.Models
{
    using System.Collections.Generic;
    using IdentityServer4.Models;

    public class Config : IConfig
    {
        public IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                // simple API with a single scope (in this case the scope name is the same as the api name)
                new ApiResource("api1", "My API"),
                new ApiResource("offline_access")
            };
        }

        public IEnumerable<IdentityResource> GetIdentityResources()
        {
            // currently we don't need any.
            var resources = new List<IdentityResource>();

            return resources;
        }

        public IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 120
                },
            };
        }
    }
}