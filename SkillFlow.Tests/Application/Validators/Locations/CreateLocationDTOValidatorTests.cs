using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Validators.Locations;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Locations
{
    public class CreateLocationDTOValidatorTests
    {
        private readonly CreateLocationDTOValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var dto = new CreateLocationDTO { LocationName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LocationName);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_Valid()
        {
            var dto = new CreateLocationDTO { LocationName = "Stockholm" };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.LocationName);
        }
    }
}
