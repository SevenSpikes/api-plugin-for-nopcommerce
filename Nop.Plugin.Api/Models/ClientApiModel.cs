namespace Nop.Plugin.Api.Models
{
    using FluentValidation.Attributes;
    using Nop.Plugin.Api.Validators;
    using Nop.Web.Framework.Mvc.Models;

    [Validator(typeof(ClientValidator))]
    public class ClientApiModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CallbackUrl { get; set; }
        public bool IsActive { get; set; }
    }
}