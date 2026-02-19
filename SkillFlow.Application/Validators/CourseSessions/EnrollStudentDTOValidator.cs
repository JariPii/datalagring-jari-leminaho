using FluentValidation;
using SkillFlow.Application.DTOs.CourseSessions;

namespace SkillFlow.Application.Validators.CourseSessions
{
    public sealed class EnrollStudentDTOValidator : AbstractValidator<EnrollStudentDTO>
    {
        public EnrollStudentDTOValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.StudentId)
                .NotEmpty()
                .WithMessage("StudentId is required");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
