using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Domain.Enums;

namespace SkillFlow.Application.Validators.CourseSessions
{
    public sealed class UpdateEnrollmetnStatusDTOValidator : AbstractValidator<UpdateEnrollmentStatusDTO>
    {
        public UpdateEnrollmetnStatusDTOValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.NewStatus)
                .Must(s => s == EnrollmentStatus.Approved || s == EnrollmentStatus.Denied)
                .WithMessage("NewStatus must be approved or denied");

            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
