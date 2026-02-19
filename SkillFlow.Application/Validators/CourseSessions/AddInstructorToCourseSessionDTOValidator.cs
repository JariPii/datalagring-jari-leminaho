using FluentValidation;
using SkillFlow.Application.DTOs.CourseSessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.CourseSessions
{
    public sealed class AddInstructorToCourseSessionDTOValidator : AbstractValidator<AddInstructorToCourseSessionDTO>
    {
        public AddInstructorToCourseSessionDTOValidator()
        {
            RuleFor(x => x.InstructorId)
                .NotEmpty()
                .WithMessage("Instructor is needed");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
