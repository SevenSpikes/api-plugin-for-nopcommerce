namespace Nop.Plugin.Api
{
    using Microsoft.AspNetCore.Authentication;
    using Nop.Services.Authentication.External;

    public class ApiAuthentication : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
        }
    }
}