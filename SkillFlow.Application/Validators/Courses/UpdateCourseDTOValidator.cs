using FluentValidation;
using SkillFlow.Application.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Courses
{
    public sealed class UpdateCourseDTOValidator : AbstractValidator<UpdateCourseDTO>
    {
        public UpdateCourseDTOValidator()
        {
            RuleFor(x => x.CourseName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.CourseName is not null);

            RuleFor(x => x.CourseDescription)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(150)
                .When(x => x.CourseDescription is not null);

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
