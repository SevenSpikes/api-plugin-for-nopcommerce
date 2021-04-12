﻿using System;
using System.Collections.Generic;
using FluentValidation;
using JetBrains.Annotations;
using Nop.Plugin.Api.DTO.SpecificationAttributes;

namespace Nop.Plugin.Api.Validators
{
    [UsedImplicitly]
    public class SpecificationAttributeDtoValidator : AbstractValidator<SpecificationAttributeDto>
    {
        public SpecificationAttributeDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePairs)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                //apply "create" rules
                RuleFor(x => x.Id).Equal(0).WithMessage("id must be zero or null for new records");

                ApplyNameRule();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                //apply "update" rules
                RuleFor(x => x.Id).GreaterThan(0).WithMessage("invalid id");
                ApplyNameRule();
            }
        }

        private void ApplyNameRule()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("invalid name");
        }
    }
}
