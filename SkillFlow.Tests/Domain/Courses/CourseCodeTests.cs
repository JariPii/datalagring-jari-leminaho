using FluentAssertions;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Domain.Courses
{
    public class CourseCodeTests
    {
        [Fact]
        public void Create_ShouldBuildValue_WithSuffixPadding()
        {
            var code = CourseCode.Create("Math", CourseType.BAS, 10);

            code.Value.Should().EndWith("-010");
            code.CourseType.Should().Be(CourseType.BAS);
            code.CourseSuffix.Should().Be(10);
            code.Value.Length.Should().Be(10);
        }

        [Fact]
        public void FromValue_WithValidFormat_ShouldParse()
        {
            var code = CourseCode.FromValue("MTHBAS-010");

            code.CoursePart.Should().Be("MTH");
            code.CourseType.Should().Be(CourseType.BAS);
            code.CourseSuffix.Should().Be(10);
            code.Value.Should().Be("MTHBAS-010");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("TOO-SHORT")]
        [InlineData("MTHBAS010")]
        [InlineData("MTHBAS_010")]
        public void FromValue_WithInvalidFormat_ShouldThrow(string? value)
        {
            var act = () => CourseCode.FromValue(value!);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateUnique_ShouldReturnFirstAvailableSuffix()
        {
            bool Exists(string v) => v is "MTHBAS-010" or "MTHBAS-020";

            var code = CourseCode.CreateUnique("Math", CourseType.BAS, Exists, startSuffix: 10);

            code.Value.Should().Be("MTHBAS-030");
        }

        [Fact]
        public void CreateUnique_WhenNoSlotsLeft_ShouldThrow()
        {
            bool Exists(string _) => true;

            var act = () => CourseCode.CreateUnique("Math", CourseType.BAS, Exists, startSuffix: 10);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("No available course codes remaining");
        }
    }
}
