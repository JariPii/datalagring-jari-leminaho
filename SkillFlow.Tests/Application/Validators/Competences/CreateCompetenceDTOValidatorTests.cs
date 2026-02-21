using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Validators.Competences;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Competences
{
    public class CreateCompetenceDTOValidatorTests
    {
        private readonly CreateCompetenceDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // Name
        // ---------------------------

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
        // helper
        // ---------------------------

        private static CreateCompetenceDTO ValidDto()
            => new()
            {
                Name = "C# Programming"
            };
    }
}
