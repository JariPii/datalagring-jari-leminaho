using FluentValidation;
using SkillFlow.Application.DTOs.Competences;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Competences
{
    public sealed class UpdateCompetenceDTOValidator : AbstractValidator<UpdateCompetenceDTO>
    {
        public UpdateCompetenceDTOValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;


            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.Name is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
