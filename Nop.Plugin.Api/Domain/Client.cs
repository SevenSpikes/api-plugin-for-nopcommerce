using Nop.Core;

namespace Nop.Plugin.Api.Domain
{
    public class Client : BaseEntity
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackUrl { get; set; }
        public bool IsActive { get; set; }
        public string AuthenticationCode { get; set; }
    }
}