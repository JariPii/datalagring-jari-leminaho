using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Validators.Competences;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Competences
{
    public class UpdateCompetenceDTOValidatorTests
    {
        private readonly UpdateCompetenceDTOValidator _validator = new();

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
            var dto = ValidDto() with { Name = null };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Name_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { Name = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Name_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { Name = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Name);
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

        private static UpdateCompetenceDTO ValidDto()
            => new()
            {
                Name = "C# Programming",
                RowVersion = new byte[] { 1, 2, 3 }
            };
    }
}
