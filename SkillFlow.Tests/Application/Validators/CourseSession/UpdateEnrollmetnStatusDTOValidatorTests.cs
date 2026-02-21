using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Validators.CourseSessions;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.CourseSession
{
    public class UpdateEnrollmetnStatusDTOValidatorTests
    {
        private readonly UpdateEnrollmetnStatusDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_WithApprovedStatus_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto() with { NewStatus = EnrollmentStatus.Approved };

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Valid_Dto_WithDeniedStatus_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto() with { NewStatus = EnrollmentStatus.Denied };

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // NewStatus
        // ---------------------------

        [Fact]
        public void NewStatus_WhenPending_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { NewStatus = EnrollmentStatus.Pending };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.NewStatus)
                  .WithErrorMessage("NewStatus must be approved or denied");
        }

        [Fact]
        public void NewStatus_WhenDefaultEnumValue_ShouldHaveError()
        {
            var dto = ValidDto() with { NewStatus = default };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.NewStatus)
                  .WithErrorMessage("NewStatus must be approved or denied");
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

        private static UpdateEnrollmentStatusDTO ValidDto()
            => new()
            {
                NewStatus = EnrollmentStatus.Approved,
                RowVersion = new byte[] { 1, 2, 3 }
            };
    }
}
