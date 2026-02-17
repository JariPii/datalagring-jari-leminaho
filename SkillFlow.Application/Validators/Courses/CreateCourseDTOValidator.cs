using FluentValidation;
using SkillFlow.Application.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Courses
{
    public sealed class CreateCourseDTOValidator : AbstractValidator<CreateCourseDTO>
    {
        public CreateCourseDTOValidator()
        {
            RuleFor(x => x.CourseName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.CourseDescription)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.CourseType)
                .IsInEnum()
                .WithMessage("Invalid course type");
        }
    }
}
