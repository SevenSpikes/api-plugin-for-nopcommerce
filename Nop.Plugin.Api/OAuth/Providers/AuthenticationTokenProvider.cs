using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Owin.OAuth.Providers
{
    public class AuthenticationTokenProvider : IAuthenticationTokenProvider
    {
        public Action<AuthenticationTokenCreateContext> OnCreate { get; set; }
        public Func<AuthenticationTokenCreateContext, Task> OnCreateAsync { get; set; }
        public Action<AuthenticationTokenReceiveContext> OnReceive { get; set; }
        public Func<AuthenticationTokenReceiveContext, Task> OnReceiveAsync { get; set; }

        private IClientService ClientService
        {
            get
            {
                // Needs to be resolved every time. If standart property injection is used there are cases in which the DbContext could be disposed.
                return EngineContext.Current.Resolve<IClientService>();
            }
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            if (OnCreateAsync != null && OnCreate == null)
            {
                throw new InvalidOperationException("Authentication failed on create");
            }
            if (OnCreateAsync != null)
            {
                await OnCreateAsync.Invoke(context);
            }
            else
            {
                Create(context);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            if (OnReceiveAsync != null && OnReceive == null)
            {
                throw new InvalidOperationException("Authentication failed on recieve");
            }
            if (OnReceiveAsync != null)
            {
                await OnReceiveAsync.Invoke(context);
            }
            else
            {
                Receive(context);
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            string clientId = context.Ticket.Properties.Dictionary["client_id"];

            if (!string.IsNullOrEmpty(clientId))
            {
                Client client = ClientService.GetClientByClientId(clientId);

                if (client != null && client.IsActive)
                {
                    client.AuthenticationCode = context.SerializeTicket();
                    ClientService.UpdateClient(client);

                    context.SetToken(client.AuthenticationCode);
                }
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            string clientId = context.Ticket.Properties.Dictionary["client_id"];

            if (!string.IsNullOrEmpty(clientId))
            {
                Client client = ClientService.GetClientByClientId(clientId);

                if (client != null && client.IsActive)
                {
                    string authenticationCode = client.AuthenticationCode;

                    client.AuthenticationCode = null;
                    ClientService.UpdateClient(client);

                    context.DeserializeTicket(authenticationCode);
                }
            }
        }
    }
}
