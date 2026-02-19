using FluentValidation;
using SkillFlow.Application.DTOs.Attendees;

namespace SkillFlow.Application.Validators.Attendees
{
    public sealed class CreateAttendeeDTOValidator : AbstractValidator<CreateAttendeeDTO>
    {
        public CreateAttendeeDTOValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(50)
                .EmailAddress();

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(30)
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role.");
        }
    }
}
