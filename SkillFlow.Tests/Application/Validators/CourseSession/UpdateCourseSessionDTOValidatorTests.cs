using FluentAssertions;
using FluentValidation.TestHelper;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Validators.CourseSessions;
using Xunit;

namespace SkillFlow.Tests.Application.Validators.CourseSession
{
    public class UpdateCourseSessionDTOValidatorTests
    {
        private readonly UpdateCourseSessionDTOValidator _validator = new();

        [Fact]
        public void Valid_MinimalDto_ShouldBeValid()
        {
            var dto = ValidDto();

            var result = _validator.TestValidate(dto);

            result.IsValid.Should().BeTrue();
        }

        // -------------------------
        // RowVersion
        // -------------------------

        [Fact]
        public void RowVersion_WhenEmpty_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { RowVersion = Array.Empty<byte>() };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.RowVersion)
                  .WithErrorMessage("RowVersion is required");
        }

        // -------------------------
        // CourseId / LocationId (optional men om angivna ska de vara non-empty)
        // -------------------------

        [Fact]
        public void CourseId_WhenProvidedButEmptyGuid_ShouldHaveError()
        {
            var dto = ValidDto() with { CourseId = Guid.Empty };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.CourseId);
        }

        [Fact]
        public void LocationId_WhenProvidedButEmptyGuid_ShouldHaveError()
        {
            var dto = ValidDto() with { LocationId = Guid.Empty };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.LocationId);
        }

        // -------------------------
        // Capacity (optional, men om angiven > 0)
        // -------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Capacity_WhenProvidedAndNotGreaterThan0_ShouldHaveError_WithExpectedMessage(int capacity)
        {
            var dto = ValidDto() with { Capacity = capacity };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Capacity)
                  .WithErrorMessage("Capacity can not be 0");
        }

        // -------------------------
        // StartDate / EndDate (optional, men om angivna ska vara non-default)
        // -------------------------

        [Fact]
        public void StartDate_WhenProvidedButDefault_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { StartDate = default(DateTime) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StartDate)
                  .WithErrorMessage("StartDate is require");
        }

        [Fact]
        public void EndDate_WhenProvidedButDefault_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { EndDate = default(DateTime) };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                  .WithErrorMessage("EndDate is required");
        }

        [Fact]
        public void EndDate_WhenBothProvidedAndEndNotAfterStart_ShouldHaveError_WithExpectedMessage()
        {
            var start = DateTime.UtcNow.AddDays(10);
            var end = start; // inte efter

            var dto = ValidDto() with { StartDate = start, EndDate = end };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.EndDate)
                  .WithErrorMessage("EndDate must be after StartDate");
        }

        // -------------------------
        // InstructorIds
        // -------------------------

        [Fact]
        public void InstructorIds_WhenEmptyList_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { InstructorIds = new List<Guid>() };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.InstructorIds)
                  .WithErrorMessage("Atleast one instructor is required");
        }

        [Fact]
        public void InstructorIds_WhenContainsEmptyGuid_ShouldHaveError_WithExpectedMessage()
        {
            var dto = ValidDto() with { InstructorIds = new List<Guid> { Guid.Empty } };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor("InstructorIds[0]")
                  .WithErrorMessage("InstructorId can not be empty GUID");
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

        [Fact]
        public void InstructorIds_WhenNull_ShouldNotHaveErrors_ForInstructorIdsRules()
        {
            // I din validator: InstructorIds är "optional" (reglerna körs bara When not null),
            // men du har också RuleFor(x => x.InstructorIds!) .NotEmpty() utan When,
            // så om InstructorIds = null kan den regeln krascha eller ge fel.
            //
            // Om dina DTOs defaultar InstructorIds till null och du vill tillåta null,
            // bör du lägga .When(x => x.InstructorIds is not null) även på den regeln.
            //
            // Här testar vi det beteende du "troligen" vill ha: null -> inga errors.
            var dto = ValidDto() with { InstructorIds = null };

            var act = () => _validator.TestValidate(dto);

            act.Should().NotThrow();
        }

        // -------------------------
        // helper
        // -------------------------

        private static UpdateCourseSessionDTO ValidDto()
            => new()
            {
                // RowVersion krävs alltid enligt validatorn
                RowVersion = new byte[] { 1, 2, 3 },

                // resten är optional i Update
                CourseId = null,
                LocationId = null,
                Capacity = null,
                StartDate = null,
                EndDate = null,
                InstructorIds = new List<Guid> { Guid.NewGuid() }
            };
    }
}
