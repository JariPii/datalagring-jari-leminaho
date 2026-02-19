using FluentValidation;
using SkillFlow.Application.DTOs.Locations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Locations
{
    public sealed class UpdateLocationDTOValidator : AbstractValidator<UpdateLocationDTO>
    {
        public UpdateLocationDTOValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.Name is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
