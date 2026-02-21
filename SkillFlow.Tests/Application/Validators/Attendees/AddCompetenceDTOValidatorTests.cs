using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.Validators.Attendees;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Attendees
{
    public class AddCompetenceDTOValidatorTests
    {
        private readonly AddCompetenceDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // CompetenceName
        // ---------------------------

        [Fact]
        public void CompetenceName_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { CompetenceName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CompetenceName);
        }

        [Fact]
        public void CompetenceName_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { CompetenceName = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CompetenceName);
        }

        // ---------------------------
        // RowVersion
        // ---------------------------

        [Fact]
        public void RowVersion_WhenNull_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { RowVersion = null };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RowVersion)
                  .WithErrorMessage("RowVersion is required.");
        }

        [Fact]
        public void RowVersion_WhenEmpty_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { RowVersion = Array.Empty<byte>() };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RowVersion)
                  .WithErrorMessage("RowVersion is required.");
        }

        // ---------------------------
        // helper
        // ---------------------------

        private static AddCompetenceDTO ValidDto()
            => new(
                "C# Programming",
                new byte[] { 1, 2, 3 }
            );
    }
}
