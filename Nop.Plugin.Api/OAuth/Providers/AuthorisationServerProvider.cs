using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Owin.OAuth.Providers
{
    public class AuthorisationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId;
            string clientSecret;
            var code = context.Parameters.Get("code");

            ApiSettings settings = EngineContext.Current.Resolve<ApiSettings>();

            if (!settings.EnableApi)
            {
                context.SetError("invalid_call", "Could not access private resources on this server!");
                context.Rejected();
            }
            else
            {
                if (!context.TryGetFormCredentials(out clientId, out clientSecret))
                {
                    context.TryGetBasicCredentials(out clientId, out clientSecret);
                }

                string grantType = context.Parameters.Get("grant_type");

                if ((!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(clientSecret) ||
                    !string.IsNullOrEmpty(code)) && !string.IsNullOrEmpty(grantType))
                {
                    IClientService clientService = EngineContext.Current.Resolve<IClientService>();

                    bool valid = false;

                    if (grantType == "refresh_token")
                    {
                        valid = clientService.ValidateClientById(clientId);
                    }
                    else
                    {
                        valid = clientService.ValidateClient(clientId, clientSecret, code);
                    }

                    if (valid)
                    {
                        Client client = clientService.GetClient(clientId);
                        //   _clientId = clientId;

                        if (client.IsActive)
                        {
                            context.OwinContext.Set("oauth:client", client);
                            context.Validated(clientId);
                        }
                    }
                    else
                    {
                        context.SetError("invalid_user", "User not active or invalid!");
                        context.Rejected();
                    }
                }
                else
                {
                    context.SetError("invalid_user", "User not active or invalid!");
                    context.Rejected();
                }
            }
        }

        // We need this method because the granting of the credentials can't work withouth Identity and AuthenticationTicket,
        // so we need to connect our Client with the Identity and AuthenicationTicket.
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            Client client = context.OwinContext.Get<Client>("oauth:client");

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //Think if we will need the callback url because all the data is contained in the token end point response.
            identity.AddClaim(new Claim("CallbackUrl", client.CallbackUrl));

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            context.Validated(ticket);

            return base.GrantResourceOwnerCredentials(context);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            IClientService clientService = EngineContext.Current.Resolve<IClientService>();

            var isValid = clientService.ValidateClientById(context.ClientId);
            
            if (isValid)
            {
                Client client = clientService.GetClient(context.ClientId);

                if (client.IsActive)
                {
                    context.Validated(client.CallbackUrl);
                }
            }

            return Task.FromResult(0);
        }
    }
}