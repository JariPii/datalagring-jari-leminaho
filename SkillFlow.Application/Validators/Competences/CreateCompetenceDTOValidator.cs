using FluentValidation;
using SkillFlow.Application.DTOs.Competences;

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
