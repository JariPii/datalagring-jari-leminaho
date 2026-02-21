using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Validators.CourseSessions;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.CourseSession
{
    public class AddInstructorToCourseSessionDTOValidatorTests
    {
        private readonly AddInstructorToCourseSessionDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // InstructorId
        // ---------------------------

        [Fact]
        public void InstructorId_WhenEmpty_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { InstructorId = Guid.Empty };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.InstructorId)
                  .WithErrorMessage("Instructor is needed");
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

        private static AddInstructorToCourseSessionDTO ValidDto()
            => new()
            {
                InstructorId = Guid.NewGuid(),
                RowVersion = new byte[] { 1, 2, 3 }
            };
    }
}
