namespace Nop.Plugin.Api
{
    using System.Collections.Generic;
    using IdentityModel;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Helpers;
    using Nop.Services.Authentication.External;

    public class ApiAuthentication : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
           var signingKey = CryptoHelper.CreateRsaSecurityKey();                        
           
            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
                {
                    jwt.Audience = "nop_api";
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = false,
                        ValidateIssuer = false,
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                        // Uncomment this if you are using an certificate to sign your tokens.
                        // IssuerSigningKey = new X509SecurityKey(cert),
                        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                               new List<RsaSecurityKey> { signingKey }
                    };
                });
        }
    }
}