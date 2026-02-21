using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.Validators.Attendees;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Attendees
{
    public class CreateAttendeeDTOValidatorTests
    {
        private readonly CreateAttendeeDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // Email
        // ---------------------------

        [Fact]
        public void Email_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { Email = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Email_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { Email = new string('a', 51) + "@x.se" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Email_WhenInvalidFormat_ShouldHaveError()
        {
            var dto = ValidDto() with { Email = "not-an-email" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        // ---------------------------
        // FirstName
        // ---------------------------

        [Fact]
        public void FirstName_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { FirstName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void FirstName_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { FirstName = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        // ---------------------------
        // LastName
        // ---------------------------

        [Fact]
        public void LastName_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { LastName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        [Fact]
        public void LastName_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { LastName = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LastName);
        }

        // ---------------------------
        // PhoneNumber (optional with When not null/whitespace)
        // ---------------------------

        [Fact]
        public void PhoneNumber_WhenNull_ShouldNotHaveError()
        {
            var dto = ValidDto() with { PhoneNumber = null };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Fact]
        public void PhoneNumber_WhenWhitespace_ShouldNotHaveError()
        {
            var dto = ValidDto() with { PhoneNumber = "   " };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
        }

        [Fact]
        public void PhoneNumber_WhenTooLongAndProvided_ShouldHaveError()
        {
            var dto = ValidDto() with { PhoneNumber = new string('1', 31) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
        }

        // ---------------------------
        // Role
        // ---------------------------

        [Fact]
        public void Role_WhenInvalidEnumValue_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { Role = (Role)999 }; // byt enum-typ om din heter annorlunda

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Role)
                  .WithErrorMessage("Invalid role.");
        }

        // ---------------------------
        // helper
        // ---------------------------

        private static CreateAttendeeDTO ValidDto()
            => new()
            {
                Email = "test@example.com",
                FirstName = "Anna",
                LastName = "Svensson",
                PhoneNumber = "0701234567",
                Role = Role.Student // byt till en giltig enum i ditt projekt
            };
    }
}
