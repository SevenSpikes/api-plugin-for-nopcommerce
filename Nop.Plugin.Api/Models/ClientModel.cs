using FluentValidation.Attributes;
using Nop.Plugin.Api.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Api.Models
{
    [Validator(typeof(ClientValidator))]
    public class ClientModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Plugins.Api.Admin.Client.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Plugins.Api.Admin.Client.ClientId")]
        public string ClientId { get; set; }
        [NopResourceDisplayName("Plugins.Api.Admin.Client.ClientSecret")]
        public string ClientSecret { get; set; }
        [NopResourceDisplayName("Plugins.Api.Admin.Client.CallbackUrl")]
        public string CallbackUrl { get; set; }
        [NopResourceDisplayName("Plugins.Api.Admin.Client.IsActive")]
        public bool IsActive { get; set; }
    }
}