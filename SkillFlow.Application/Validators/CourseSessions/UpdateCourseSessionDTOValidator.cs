using FluentValidation;
using SkillFlow.Application.DTOs.CourseSessions;

namespace SkillFlow.Application.Validators.CourseSessions
{
    public sealed class UpdateCourseSessionDTOValidator : AbstractValidator<UpdateCourseSessionDTO>
    {
        public UpdateCourseSessionDTOValidator()
        {
            RuleFor(x => x.CourseId)
                .Must(id => id != Guid.Empty)
                .When(x => x.CourseId is not null)
                .WithMessage("CourseId can not be empty GUID");

            RuleFor(x => x.LocationId)
                .Must(id => id != Guid.Empty)
                .When(x => x.LocationId is not null)
                .WithMessage("LocationId can not be empty GUID");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Capacity can not be 0")
                .When(x => x.Capacity is not null);

            RuleFor(x => x.StartDate)
                .Must(d => d != default(DateTime))
                .WithMessage("StartDate is require")
                .When(x => x.StartDate is not null);

            RuleFor(x => x.EndDate)
                .Must(d => d != default(DateTime))
                .WithMessage("EndDate is required")
                .When(x => x.EndDate is not null);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate)
                .WithMessage("EndDate must be after StartDate")
                .When(x => x.StartDate is not null && x.EndDate is not null
                            && x.StartDate != default(DateTime) && x.EndDate != default(DateTime));

            RuleFor(x => x.InstructorIds)
                .NotEmpty()
                .WithMessage("Atleast one instructor is required")
                .When(x => x.InstructorIds is not null);

            RuleForEach(x => x.InstructorIds)
                .NotEmpty()
                .WithMessage("InstructorId can not be empty GUID")
                .When(x => x.InstructorIds is not null);

            RuleFor(x => x.InstructorIds)
                .Must(ids => ids!.Distinct().Count() == ids.Count)
                .WithMessage("InstructorsIds must be unique")
                .When(x => x.InstructorIds is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
