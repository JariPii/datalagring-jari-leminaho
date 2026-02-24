using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Validators.Locations;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Locations
{
    public class UpdateLocationDTOValidatorTests
    {
        private readonly UpdateLocationDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // Name (optional with When not null)
        // ---------------------------

        [Fact]
        public void Name_WhenNull_ShouldNotHaveError()
        {
            var dto = ValidDto() with { LocationName = null };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.LocationName);
        }

        [Fact]
        public void Name_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { LocationName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LocationName);
        }

        [Fact]
        public void Name_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { LocationName = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LocationName);
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
                  .WithErrorMessage("RowVersion is required");
        }

        [Fact]
        public void RowVersion_WhenEmpty_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { RowVersion = Array.Empty<byte>() };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RowVersion)
                  .WithErrorMessage("RowVersion is required");
        }

        // ---------------------------
        // helper
        // ---------------------------

        private static UpdateLocationDTO ValidDto()
            => new()
            {
                LocationName = "Stockholm",
                RowVersion = new byte[] { 1, 2, 3 }
            };
    }
}
