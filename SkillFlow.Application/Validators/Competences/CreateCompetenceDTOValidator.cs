using FluentValidation;
using SkillFlow.Application.DTOs.Competences;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Competences
{
    public sealed class CreateCompetenceDTOValidator : AbstractValidator<CreateCompetenceDTO>
    {
        public CreateCompetenceDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
