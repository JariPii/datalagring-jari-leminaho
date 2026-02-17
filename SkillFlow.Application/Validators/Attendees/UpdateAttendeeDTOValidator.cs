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

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .EmailAddress()
                .When(x => x.Email is not null);


            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.FirstName is not null);


            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(50)
                .When(x => x.LastName is not null);


            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MaximumLength(30)
                .When(x => x.PhoneNumber is not null);


            RuleFor(x => x.RowVersion)
                .NotEmpty()
                .WithMessage("RowVersion is required");
        }
    }
}
