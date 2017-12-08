namespace Nop.Plugin.Api.Attributes
{
    using Microsoft.AspNetCore.Authorization;
    using Nop.Core.Plugins;

    // We need the ApiAuthorize attribute because when the api plugin assembly is loaded in memory by PluginManager 
    // all of its attributes are being initialized by the .NetFramework.
    // The authorize attribute of the api plugin is marked with the Bearer authentication scheme, but the scheme is registered in the ApiStartup class,
    // which is called on plugin install. 
    // If the plugin is not installed the authorize attribute will still be initialized when the assembly is loaded in memory, but the scheme won't be registered,
    // which will cause an exception.
    // That is why we need to make sure that the plugin is installed before setting the scheme.
    public class ApiAuthorize : AuthorizeAttribute
    {
        public string Policy
        {
            get
            {
                return base.AuthenticationSchemes;
            }
            set
            {
                base.AuthenticationSchemes = GetAuthenticationSchemeName(value);
            }
        }

        public string AuthenticationSchemes
        {
            get
            {
                return base.AuthenticationSchemes;
            }
            set
            {
                base.AuthenticationSchemes = GetAuthenticationSchemeName(value);
            }
        }
        
        private string GetAuthenticationSchemeName(string value)
        {
            bool pluginInstalled = PluginManager.FindPlugin(typeof(ApiStartup))?.Installed ?? false;

            if (pluginInstalled)
            {
                return value;
            }

            return default(string);
        }
    }
}