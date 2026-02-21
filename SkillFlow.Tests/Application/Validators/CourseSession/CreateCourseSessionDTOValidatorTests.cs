using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Validators.CourseSessions;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.CourseSession
{
    public class CreateCourseSessionDTOValidatorTests
    {
        private readonly CreateCourseSessionDTOValidator _validator = new();

        [Fact]
        public void Valid_Dto_Should_Not_Have_Any_Errors()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // ---------------------------
        // CourseCode
        // ---------------------------

        [Fact]
        public void CourseCode_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseCode = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseCode);
        }

        [Fact]
        public void CourseCode_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseCode = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseCode);
        }

        // ---------------------------
        // LocationName
        // ---------------------------

        [Fact]
        public void LocationName_WhenEmpty_ShouldHaveError()
        {
            var dto = ValidDto() with { LocationName = "" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LocationName);
        }

        [Fact]
        public void LocationName_WhenTooLong_ShouldHaveError()
        {
            var dto = ValidDto() with { LocationName = new string('A', 51) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LocationName);
        }

        // ---------------------------
        // StartDate
        // ---------------------------

        [Fact]
        public void StartDate_WhenDefault_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { StartDate = default };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StartDate)
                  .WithErrorMessage("StartDate is required");
        }

        // ---------------------------
        // EndDate
        // ---------------------------

        [Fact]
        public void EndDate_WhenDefault_ShouldHaveError()
        {
            var dto = ValidDto() with { EndDate = default };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EndDate);
        }

        [Fact]
        public void EndDate_WhenNotAfterStartDate_ShouldHaveError_WithExpectedMessage()
        {
            var start = DateTime.UtcNow.AddDays(10);
            var dto = ValidDto() with { StartDate = start, EndDate = start };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                  .WithErrorMessage("EndDate must be after StartDate");
        }

        // ---------------------------
        // Capacity
        // ---------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Capacity_WhenNotGreaterThan0_ShouldHaveError_WithExpectedMessage(int capacity)
        {
            var dto = ValidDto() with { Capacity = capacity };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Capacity)
                  .WithErrorMessage("Capacity must be greater than 0");
        }

        // ---------------------------
        // InstructorIds (collection)
        // ---------------------------

        [Fact]
        public void InstructorIds_WhenNull_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { InstructorIds = null };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.InstructorIds)
                  .WithErrorMessage("At least one instructor is required");
        }

        [Fact]
        public void InstructorIds_WhenEmptyList_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { InstructorIds = new List<Guid>() };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.InstructorIds)
                  .WithErrorMessage("At least one instructor is required");
        }

        [Fact]
        public void InstructorIds_WhenContainsEmptyGuid_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { InstructorIds = new List<Guid> { Guid.Empty } };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor("InstructorIds[0]")
                  .WithErrorMessage("InstructorIds can not be empty GUID");
        }

        [Fact]
        public void InstructorIds_WhenContainsDuplicates_ShouldHaveError_WithExpectedMessage()
        {
            var same = Guid.NewGuid();
            var dto = ValidDto() with { InstructorIds = new List<Guid> { same, same } };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.InstructorIds)
                  .WithErrorMessage("InstructorsIds must be unique");
        }

        // ---------------------------
        // helper
        // ---------------------------

        private static CreateCourseSessionDTO ValidDto()
            => new()
            {
                CourseCode = "MTHBAS-010",
                LocationName = "Stockholm",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                Capacity = 10,
                InstructorIds = new List<Guid> { Guid.NewGuid() }
            };
    }
}
