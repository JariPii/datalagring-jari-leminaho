using FluentValidation;
using SkillFlow.Application.DTOs.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Locations
{
    public sealed class CreateLocationDTOValidator : AbstractValidator<CreateLocationDTO>
    {
        public CreateLocationDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
