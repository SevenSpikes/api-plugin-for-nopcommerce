using Nop.Plugin.Api.Domain;

namespace Nop.Plugin.Api.Helpers
{
    public interface IAuthorizationHelper
    {
        bool ClientExistsAndActive();
        Client GetCurrentClientFromClaims();
    }
}