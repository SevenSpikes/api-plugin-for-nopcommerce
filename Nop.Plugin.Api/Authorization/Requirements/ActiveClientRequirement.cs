namespace Nop.Plugin.Api.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;

    public class ActiveClientRequirement : IAuthorizationRequirement
    {
        public bool IsClientActive()
        {
            //if (!_authorizationHelper.ClientExistsAndActive())
            //{
            //    // don't authorize if any of the above is not true
            //    return false;
            //}

            return true;
        }
    }
}