using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Helpers
{
    public class AuthorizationHelper : IAuthorizationHelper
    {
        public bool ClientExistsAndActive()
        {
            Client client = GetCurrentClientFromClaims();

            if (client != null && client.IsActive)
            {
                return true;
            }

            return false;
        }

        public Client GetCurrentClientFromClaims()
        {
            // This needs to be here, because otherwise we might get 
            // "Operation cannot be completed, because the DbContext has been disposed" exception.
            var clientService = EngineContext.Current.Resolve<IClientService>();

            Client client = null;

            var user = HttpContext.Current.GetOwinContext().Authentication.User;

            if (user != null)
            {
                IList<Claim> claims = user.Claims.ToList();

                Claim clientIdClaim = claims.FirstOrDefault(x => x.Type == "client_id");

                if (clientIdClaim != null)
                {
                    string clientId = clientIdClaim.Value;

                    if (!string.IsNullOrEmpty(clientId))
                    {
                        client = clientService.GetClientByClientId(clientId);
                    }
                }
            }

            return client;
        }
    }
}