using FluentValidation;
using SkillFlow.Application.DTOs.Locations;

namespace SkillFlow.Application.Validators.Locations
{
    public sealed class UpdateLocationDTOValidator : AbstractValidator<UpdateLocationDTO>
    {
        public UpdateLocationDTOValidator()
        {
            RuleFor(x => x.LocationName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.LocationName is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
