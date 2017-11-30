namespace Nop.Plugin.Api
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Nop.Plugin.Api.Helpers;
    using Nop.Services.Authentication.External;

    public class ApiAuthentication : IExternalAuthenticationRegistrar
    {
        public void Configure(AuthenticationBuilder builder)
        {
            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            builder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwt =>
                {
                    jwt.Audience = "nop_api";
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateActor = false,
                        ValidateIssuer = false,
                        // Uncomment this if you are using an certificate to sign your tokens.
                        // IssuerSigningKey = new X509SecurityKey(cert),
                        IssuerSigningKey = signingKey,
                        IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid,
                                TokenValidationParameters validationParameters) =>
                                new List<RsaSecurityKey> { signingKey }
                        // Uncomment this if you are using an certificate to sign your tokens.
                        //IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid,
                        //        TokenValidationParameters validationParameters) =>
                        //        new List<X509SecurityKey> { new X509SecurityKey(cert) }
                    };
                });
        }
    }
}