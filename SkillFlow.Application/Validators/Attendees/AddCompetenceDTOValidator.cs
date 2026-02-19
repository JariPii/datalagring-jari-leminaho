using FluentValidation;
using SkillFlow.Application.DTOs.Attendees;

namespace SkillFlow.Application.Validators.Attendees
{
    public sealed class AddCompetenceDTOValidator : AbstractValidator<AddCompetenceDTO>
    {
        public AddCompetenceDTOValidator()
        {
            RuleFor(x => x.CompetenceName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required.");
        }
    }
}
