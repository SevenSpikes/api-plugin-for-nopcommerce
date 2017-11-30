namespace Nop.Plugin.Api.IdentityServer.Models
{
    using System.Collections.Generic;
    using IdentityServer4.Models;

    public interface IConfig
    {
        IEnumerable<ApiResource> GetApiResources();

        IEnumerable<IdentityResource> GetIdentityResources();

        IEnumerable<Client> GetClients();
    }
}