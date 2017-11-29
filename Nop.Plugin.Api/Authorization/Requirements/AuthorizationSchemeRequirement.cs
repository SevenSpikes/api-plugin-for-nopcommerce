namespace Nop.Plugin.Api.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    public class AuthorizationSchemeRequirement : IAuthorizationRequirement
    {
        public bool IsValid(IHeaderDictionary requestHeaders)
        {
            if (requestHeaders != null && 
                requestHeaders.ContainsKey("Authorization") && 
                requestHeaders["Authorization"].ToString().Contains("Bearer"))
            {
                return true;
            }

            return false;
        }
    }
}