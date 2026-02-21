using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Validators.Courses;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Courses
{
    public class CreateCourseDTOValidatorTests
    {
        private readonly CreateCourseDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // CourseName
        // ---------------------------

        [Fact]
        public void CourseName_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseName);
        }

        [Fact]
        public void CourseName_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseName = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseName);
        }

        // ---------------------------
        // CourseDescription
        // ---------------------------

        [Fact]
        public void CourseDescription_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseDescription = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseDescription);
        }

        [Fact]
        public void CourseDescription_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseDescription = new string('A', 151) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseDescription);
        }

        // ---------------------------
        // CourseType
        // ---------------------------

        [Fact]
        public void CourseType_WhenInvalidEnumValue_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { CourseType = (CourseType)999 };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseType)
                  .WithErrorMessage("Invalid course type");
        }

        // ---------------------------
        // helper
        // ---------------------------

        private static CreateCourseDTO ValidDto()
            => new()
            {
                CourseName = "C# Fundamentals",
                CourseDescription = "Learn the basics of C# and .NET",
                CourseType = CourseType.BAS // ändra till giltig enum från ditt projekt
            };
    }
}
