using FluentValidation;
using Nop.Plugin.Api.Models;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Validators
{
    public class ClientValidator : AbstractValidator<ClientModel>
    {
        public ClientValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.Name"));
            RuleFor(x => x.ClientId).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientId"));
            RuleFor(x => x.ClientSecret).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.ClientSecret"));
            RuleFor(x => x.CallbackUrl).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Api.Admin.Entities.Client.FieldValidationMessages.CallbackUrl"));
        }
    }
}