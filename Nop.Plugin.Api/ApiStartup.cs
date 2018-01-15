namespace Nop.Plugin.Api
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using IdentityServer4.EntityFramework.DbContexts;
    using IdentityServer4.EntityFramework.Entities;
    using IdentityServer4.Models;
    using Microsoft.AspNet.WebHooks.Diagnostics;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Rewrite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nop.Core;
    using Nop.Core.Data;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Authorization.Policies;
    using Nop.Plugin.Api.Authorization.Requirements;
    using Nop.Plugin.Api.Constants;
    using Nop.Plugin.Api.Helpers;
    using Nop.Plugin.Api.IdentityServer.Endpoints;
    using Nop.Plugin.Api.IdentityServer.Generators;
    using Nop.Plugin.Api.IdentityServer.Middlewares;
    using Nop.Plugin.Api.WebHooks;
    using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;

    public class ApiStartup : INopStartup
    {
        // TODO: extract all methods into extensions.
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            AddRequiredConfiguration();

            AddBindingRedirectsFallbacks();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            
            AddTokenGenerationPipeline(services);

            AddAuthorizationPipeline(services);
        }
      
        public void Configure(IApplicationBuilder app)
        {
            // The default route templates for the Swagger docs and swagger - ui are "swagger/docs/{apiVersion}" and "swagger/ui/index#/{assetPath}" respectively.
            //app.UseSwagger();
            //app.UseSwaggerUI(options =>
            //    {
            //        //var currentAssembly = Assembly.GetAssembly(this.GetType());
            //        //var currentAssemblyName = currentAssembly.GetName().Name;

            //        //Needeed for removing the "Try It Out" button from the post and put methods.
            //        //http://stackoverflow.com/questions/36772032/swagger-5-2-3-supportedsubmitmethods-removed/36780806#36780806

            //        //options.InjectOnCompleteJavaScript($"{currentAssemblyName}.Scripts.swaggerPostPutTryItOutButtonsRemoval.js");

            //        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //    }
            //);

            // This needs to be called here because in the plugin install method identity server is not yet registered.
            ApplyIdentityServerMigrations(app);

            SeedData(app);
            
            var rewriteOptions = new RewriteOptions()
                .AddRedirect("oauth/(.*)", "connect/$1", 307)
                .AddRedirect("api/token", "connect/token", 307);

            app.UseRewriter(rewriteOptions);

            app.UseMiddleware<IdentityServerScopeParameterMiddleware>();

            ////uncomment only if the client is an angular application that directly calls the oauth endpoint
            //// app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseAuthentication();
            app.UseIdentityServer();
        }

        private void AddRequiredConfiguration()
        {
            var configManagerHelper = new NopConfigManagerHelper();

            // some of third party libaries that we use for WebHooks and Swagger use older versions
            // of certain assemblies so we need to redirect them to the once that nopCommerce uses
            configManagerHelper.AddBindingRedirects();

            // required by the WebHooks support
            configManagerHelper.AddConnectionString();

            var dataSettings = configManagerHelper.DataSettings;
            Microsoft.AspNet.WebHooks.Config.SettingsDictionary settings = new Microsoft.AspNet.WebHooks.Config.SettingsDictionary();
            settings.Add("MS_SqlStoreConnectionString", dataSettings.DataConnectionString);
            settings.Connections.Add("MS_SqlStoreConnectionString", new Microsoft.AspNet.WebHooks.Config.ConnectionSettings("MS_SqlStoreConnectionString", dataSettings.DataConnectionString));

            ILogger logger = new NopWebHooksLogger();
            Microsoft.AspNet.WebHooks.IWebHookStore store = new Microsoft.AspNet.WebHooks.SqlWebHookStore(settings, logger);

            Microsoft.AspNet.WebHooks.Services.CustomServices.SetStore(store);

            // This is required only in development.
            // It it is required only when you want to send a web hook to an https address with an invalid SSL certificate. (self-signed)
            // The code marks all certificates as valid.
            // We may want to extract this as a setting in the future.

            // NOTE: If this code is commented the certificates will be validated.
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
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
            X509Certificate2 cert = CryptoHelper.GetTokenSigningCertificate();

            // This is in case you had the plugin installed in some prev version of nopCommerce.
            if (cert == null)
            {
                CryptoHelper.CreateSelfSignedCertificate("nop-api-certificate");
                cert = CryptoHelper.GetTokenSigningCertificate();
            }

            DataSettingsManager dataSettingsManager = new DataSettingsManager();

            DataSettings dataSettings = dataSettingsManager.LoadSettings();
            string connectionStringFromNop = dataSettings.DataConnectionString;

            var migrationsAssembly = typeof(ApiStartup).GetTypeInfo().Assembly.GetName().Name;
            
            services.AddIdentityServer()
                .AddSigningCredential(cert)
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
                .AddAuthorizeInteractionResponseGenerator<NopApiAuthorizeInteractionResponseGenerator>()
                .AddEndpoint<AuthorizeCallbackEndpoint>("Authorize", "/oauth/authorize/callback")
                .AddEndpoint<AuthorizeEndpoint>("Authorize", "/oauth/authorize")
                .AddEndpoint<TokenEndpoint>("Token", "/oauth/token");
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

                    TryRunUpgradeScript(configurationContext);
                }
            }
        }

        private string LoadUpgradeScript()
        {
            string path = CommonHelper.MapPath("~/Plugins/Nop.Plugin.Api/upgrade_script.sql");
            string script = File.ReadAllText(path);

            return script;
        }

        private void TryRunUpgradeScript(ConfigurationDbContext configurationContext)
        {
            try
            {
                // If there are no api resources we can assume that this is the first start after the upgrade and run the upgrade script.
                string upgradeScript = LoadUpgradeScript();
                configurationContext.Database.ExecuteSqlCommand(upgradeScript);

                // All client secrets must be hashed otherwise the identity server validation will fail.
                var allClients =
                    Enumerable.ToList(configurationContext.Clients.Include(client => client.ClientSecrets));
                foreach (var client in allClients)
                {
                    foreach (var clientSecret in client.ClientSecrets)
                    {
                        clientSecret.Value = HashExtensions.Sha256(clientSecret.Value);
                    }

                    client.AccessTokenLifetime = Configurations.DefaultAccessTokenExpiration;
                    client.AbsoluteRefreshTokenLifetime = Configurations.DefaultRefreshTokenExpiration;
                }

                configurationContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Probably the upgrade script was already executed and we don't need to do anything.
            }
        }

        public void AddBindingRedirectsFallbacks()
        {
            // If no binding redirects are present in the config file then this will perform the binding redirect
            RedirectAssembly("Microsoft.AspNetCore.DataProtection.Abstractions", new Version(2, 0, 0, 0), "adb9793829ddae60");
        }

        ///<summary>Adds an AssemblyResolve handler to redirect all attempts to load a specific assembly name to the specified version.</summary>
        public static void RedirectAssembly(string shortName, Version targetVersion, string publicKeyToken)
        {
            ResolveEventHandler handler = null;

            handler = (sender, args) =>
            {
                // Use latest strong name & version when trying to load SDK assemblies
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;
                
                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(new AssemblyName("x, PublicKeyToken=" + publicKeyToken).GetPublicKeyToken());
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= handler;

                return Assembly.Load(requestedAssembly);
            };
            AppDomain.CurrentDomain.AssemblyResolve += handler;
        }

        public int Order { get; }       
    }
}