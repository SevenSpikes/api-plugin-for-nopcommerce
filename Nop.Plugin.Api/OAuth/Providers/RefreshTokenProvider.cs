using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Infrastructure;
using Nop.Plugin.Api.Constants;

namespace Nop.Plugin.Api.Owin.OAuth.Providers
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public void Create(AuthenticationTokenCreateContext context)
        {
            context.SetToken(context.SerializeTicket());
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.IssuedUtc = DateTime.UtcNow;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(Configurations.RefreshTokenExpirationMinutes);

            context.SetToken(context.SerializeTicket());
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            return Task.FromResult<object>(null);
        }
    }
}
