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
                .NotNull()
                .Must(rv => rv.Length > 0)
                .WithMessage("RowVersion is required.");
        }
    }
}
