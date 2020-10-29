using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Authorization.Policies;
using Nop.Plugin.Api.Authorization.Requirements;
using Nop.Plugin.Api.Configuration;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Api.Infrastructure
{
    public class ApiStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var apiConfigSection = configuration.GetSection("Api");

            if (apiConfigSection != null)
            {
                var apiConfig = services.ConfigureStartupConfig<ApiConfiguration>(apiConfigSection);

                if (!string.IsNullOrEmpty(apiConfig.SecurityKey))
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
                    {

                        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiConfig.SecurityKey)),
                            ValidateIssuer = false, // ValidIssuer = "The name of the issuer",
                            ValidateAudience = false, // ValidAudience = "The name of the audience",
                            ValidateLifetime = true, // validate the expiration and not before values in the token
                            ClockSkew = TimeSpan.FromMinutes(apiConfig.AllowedClockSkewInMinutes)
                        };
                    });

                    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    AddAuthorizationPipeline(services);

                }
            }

        }

        public void Configure(IApplicationBuilder app)
        {
            var rewriteOptions = new RewriteOptions()
                .AddRewrite("api/token", "/token", true);

            app.UseRewriter(rewriteOptions);

            app.UseCors(x => x.AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader());

            
            app.MapWhen(context => context.Request.Path.StartsWithSegments(new PathString("/api")),
                a => 
                {

                    a.Use(async (context, next) =>
                    {
                        Console.WriteLine("API Call");
                        context.Request.EnableBuffering();
                        await next();
                    });

                    //a.UseExceptionHandler("/api/error/500/Error");

                    a
                    .UseRouting()
                    .UseAuthentication()
                    .UseAuthorization()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });

                }
            );
        }

        public int Order => 1;

        private static void AddAuthorizationPipeline(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            }).Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
                                  policy =>
                                  {
                                      policy.Requirements.Add(new ActiveApiPluginRequirement());
                                      policy.Requirements.Add(new AuthorizationSchemeRequirement());
                                      policy.Requirements.Add(new CustomerRoleRequirement());
                                      policy.RequireAuthenticatedUser();
                                  });
            });

            services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, CustomerRoleAuthorizationPolicy>();

        }
    }
}
