namespace Nop.Plugin.Api
{
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Reflection;
    using IdentityServer4.EntityFramework.DbContexts;
    using IdentityServer4.EntityFramework.Entities;
    using IdentityServer4.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Nop.Core;
    using Nop.Core.Data;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Authorization.Policies;
    using Nop.Plugin.Api.Authorization.Requirements;
    using Nop.Plugin.Api.Helpers;
    using Nop.Plugin.Api.IdentityServer.Generators;
    using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

    public class ApiStartup : INopStartup
    {
        // TODO: extract all methods into extensions.
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            AddTokenGenerationPipeline(services);

            AddAuthorizationPipeline(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            // This needs to be called here because in the plugin install method identity server is not yet registered.
            ApplyIdentityServerMigrations(app);

            SeedData(app);

            ////uncomment only if the client is an angular application that directly calls the oauth endpoint
            //// app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseAuthentication();
            app.UseIdentityServer();

            //// The default route templates for the Swagger docs and swagger - ui are "swagger/docs/{apiVersion}" and "swagger/ui/index#/{assetPath}" respectively.
            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //    {
            //        var currentAssembly = Assembly.GetAssembly(this.GetType());
            //        var currentAssemblyName = currentAssembly.GetName().Name;

            //        //         Needeed for removing the "Try It Out" button from the post and put methods.
            //        //         http://stackoverflow.com/questions/36772032/swagger-5-2-3-supportedsubmitmethods-removed/36780806#36780806

            //        options.InjectOnCompleteJavaScript($"{currentAssemblyName}.Scripts.swaggerPostPutTryItOutButtonsRemoval.js");
            //    }
            //);
        }
        
        private void AddAuthorizationPipeline(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
                    policy =>
                    {
                        policy.Requirements.Add(new ActiveApiPluginRequirement());
                        policy.Requirements.Add(new AuthorizationSchemeRequirement());
                        policy.Requirements.Add(new ActiveClientRequirement());
                        policy.Requirements.Add(new RequestFromSwaggerOptional());
                        policy.RequireAuthenticatedUser();
                    });
            });

            services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, ActiveClientAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, RequestsFromSwaggerAuthorizationPolicy>();
        }

        private void AddTokenGenerationPipeline(IServiceCollection services)
        {
            // The recomended way to sign a JWT is using a verified certificate!
            // You can use the bellow commented code to change the signing credentials.
            //IServiceProvider serviceProvider = services.BuildServiceProvider();
            //var config = serviceProvider.GetService<IConfig>();
            //X509Certificate2 cert = config.GetTokenSigningCertificate();

            DataSettingsManager dataSettingsManager = new DataSettingsManager();

            DataSettings dataSettings = dataSettingsManager.LoadSettings();
            string connectionStringFromNop = dataSettings.DataConnectionString;

            var migrationsAssembly = typeof(ApiStartup).GetTypeInfo().Assembly.GetName().Name;

            RsaSecurityKey signingKey = CryptoHelper.CreateRsaSecurityKey();

            services.AddIdentityServer()
                .AddSigningCredential(signingKey)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionStringFromNop,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionStringFromNop,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddAuthorizeInteractionResponseGenerator<NopApiAuthorizeInteractionResponseGenerator>();
        }

        private void ApplyIdentityServerMigrations(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // the database.Migrate command will apply all pending migrations and will create the database if it is not created already.
                var persistedGrantContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                persistedGrantContext.Database.Migrate();

                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationContext.Database.Migrate();
            }
        }

        private void SeedData(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var configurationContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!DynamicQueryable.Any(configurationContext.ApiResources))
                {
                    // In the simple case an API has exactly one scope. But there are cases where you might want to sub-divide the functionality of an API, and give different clients access to different parts. 
                    configurationContext.ApiResources.Add(new ApiResource()
                    {
                        Enabled = true,
                        Scopes = new List<ApiScope>()
                        {
                            new ApiScope()
                            {
                                Name = "nop_api",
                                DisplayName = "nop_api"
                            }
                        },
                        Name = "nop_api"
                    });

                    configurationContext.SaveChanges();

                    // If there are no api resources we can assume that this is the first start after the upgrade and run the upgrade script.
                    string upgradeScript = LoadUpgradeScript();
                    configurationContext.Database.ExecuteSqlCommand(upgradeScript);

                    // All client secrets must be hashed otherwise the identity server validation will fail.
                    var allClients = Enumerable.ToList(configurationContext.Clients.Include(client => client.ClientSecrets));
                    foreach (var client in allClients)
                    {
                        foreach (var clientSecret in client.ClientSecrets)
                        {
                            clientSecret.Value = HashExtensions.Sha256(clientSecret.Value);
                        }
                    }

                    configurationContext.SaveChanges();
                }
            }
        }

        private string LoadUpgradeScript()
        {
            string path = CommonHelper.MapPath("~/Plugins/Nop.Plugin.Api/upgrade_script.sql");
            string script = File.ReadAllText(path);

            return script;
        }

        public int Order { get; }
    }
}