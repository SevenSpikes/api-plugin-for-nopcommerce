namespace Nop.Plugin.Api.Attributes
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Nop.Core.Plugins;
    
    public class ApiAuthorize : AuthorizeAttribute
    {
        public string Policy
        {
            get
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }
        }

        public string AuthenticationSchemes
        {
            get
            {
                return JwtBearerDefaults.AuthenticationScheme;
            }
        }
    }
}