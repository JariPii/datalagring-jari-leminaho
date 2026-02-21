using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Validators.CourseSessions;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.CourseSession
{
    public class EnrollStudentDTOValidatorTests
    {
        private readonly EnrollStudentDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // StudentId
        // ---------------------------

        [Fact]
        public void StudentId_WhenEmpty_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { StudentId = Guid.Empty };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StudentId)
                  .WithErrorMessage("StudentId is required");
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
        public void RowVersion_WhenEmptyByteArray_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { RowVersion = Array.Empty<byte>() };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RowVersion)
                  .WithErrorMessage("RowVersion is required");
        }

        // ---------------------------
        // helper
        // ---------------------------

        private static EnrollStudentDTO ValidDto()
            => new()
            {
                StudentId = Guid.NewGuid(),
                RowVersion = new byte[] { 1, 2, 3 } // räcker att den är "not empty"
            };
    }
}
