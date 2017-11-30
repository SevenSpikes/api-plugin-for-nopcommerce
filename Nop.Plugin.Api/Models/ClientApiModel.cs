namespace Nop.Plugin.Api.Models
{
    public class ClientApiModel
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecretDescription { get; set; }
        public string RedirectUrl { get; set; }
        public bool Enabled { get; set; }
    }
}