using FluentValidation;
using SkillFlow.Application.DTOs.Attendees;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Validators.Attendees
{
    public sealed class UpdateAttendeeDTOValidator : AbstractValidator<UpdateAttendeeDTO>
    {
        public UpdateAttendeeDTOValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Email)
                .NotEmpty()
                .MaximumLength(50)
                .EmailAddress()
                .When(x => x.Email is not null);


            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.FirstName is not null);


            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.LastName is not null);


            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(30)
                .When(x => x.PhoneNumber is not null);


            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
