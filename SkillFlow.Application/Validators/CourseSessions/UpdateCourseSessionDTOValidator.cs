using FluentValidation;
using SkillFlow.Application.DTOs.CourseSessions;

namespace SkillFlow.Application.Validators.CourseSessions
{
    public sealed class UpdateCourseSessionDTOValidator : AbstractValidator<UpdateCourseSessionDTO>
    {
        public UpdateCourseSessionDTOValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .When(x => x.CourseId is not null);

            RuleFor(x => x.LocationId)
                .NotEmpty()
                .When(x => x.LocationId is not null);

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Capacity can not be 0")
                .When(x => x.Capacity is not not null);

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("StartDate is require")
                .When(x => x.StartDate is not null);

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("EndDate is required")
                .When(x => x.EndDate is not null);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("EndDate must be after StartDate")
                .When(x => x.StartDate is not null && x.EndDate is not null);

            RuleFor(x => x.InstructorIds)
                .NotNull()
                .When(x => x.InstructorIds is not null);

            RuleFor(x => x.InstructorIds)
                .NotEmpty()
                .WithMessage("Atleast one instructor is required")
                .When(x => x.InstructorIds is not null);

            RuleFor(x => x.InstructorIds!)
                .NotEmpty()
                .WithMessage("InstructorId can not be empty GUID");

            RuleFor(x => x.InstructorIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("InstructorsIds must be unique")
                .When(x => x.InstructorIds is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
