namespace Nop.Plugin.Api
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nop.Core.Infrastructure;
    using Nop.Plugin.Api.Authorization.Policies;
    using Nop.Plugin.Api.Authorization.Requirements;

    public class ApiStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            AddAuthorizationPipeline(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            ////uncomment only if the client is an angular application that directly calls the oauth endpoint
            //// app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseAuthentication();

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

            //app.UseMiddleware<WebhooksMiddleware>();
        }
        
        private void AddAuthorizationPipeline(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer",
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
            services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
            services.AddSingleton<IAuthorizationHandler, RequestsFromSwaggerAuthorizationPolicy>();
        }
        
        public int Order { get; }
    }
}