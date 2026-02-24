using FluentValidation;
using SkillFlow.Application.DTOs.Locations;

namespace SkillFlow.Application.Validators.Locations
{
    public sealed class CreateLocationDTOValidator : AbstractValidator<CreateLocationDTO>
    {
        public CreateLocationDTOValidator()
        {
            RuleFor(x => x.LocationName)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
