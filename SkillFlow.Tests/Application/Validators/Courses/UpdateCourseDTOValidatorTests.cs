using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Validators.Courses;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.Courses
{
    public class UpdateCourseDTOValidatorTests
    {
        private readonly UpdateCourseDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // CourseName (optional with When not null)
        // ---------------------------

        [Fact]
        public void CourseName_WhenNull_ShouldNotHaveError()
        {
            var dto = ValidDto() with { CourseName = null };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.CourseName);
        }

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
        // CourseDescription (optional with When not null)
        // ---------------------------

        [Fact]
        public void CourseDescription_WhenNull_ShouldNotHaveError()
        {
            var dto = ValidDto() with { CourseDescription = null };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.CourseDescription);
        }

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

        private static UpdateCourseDTO ValidDto()
            => new()
            {
                // båda satta för "happy-path"
                CourseName = "C# Fundamentals",
                CourseDescription = "Learn the basics of C# and .NET",
                RowVersion = new byte[] { 1, 2, 3 }
            };
    }
}
