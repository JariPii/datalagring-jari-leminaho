using FluentValidation;
using SkillFlow.Application.DTOs.CourseSessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.CourseSessions
{
    public sealed class CreateCourseSessionDTOValidator : AbstractValidator<CreateCourseSessionDTO>
    {
        public CreateCourseSessionDTOValidator()
        {
            RuleFor(x => x.CourseCode)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.LocationName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("StartDate is required");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThan(x => x.StartDate)
                .WithMessage("EndDate must be after StartDate");

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .WithMessage("Capacity must be greater than 0");

            RuleFor(x => x.InstructorIds)
                .NotNull()
                .NotEmpty()
                .WithMessage("At least one instructor is required");

            RuleForEach(x => x.InstructorIds)
                .NotEmpty()
                .WithMessage("InstructorIds can not be empty GUID");

            RuleFor(x => x.InstructorIds)
                .Must(ids => ids.Distinct().Count() == ids.Count)
                .WithMessage("InstructorsIds must be unique");
        }
    }
}
