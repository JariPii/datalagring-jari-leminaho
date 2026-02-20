using FluentAssertions;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Domain.Courses
{
    public class CourseNameTests
    {
        [Fact]
        public void Create_ShouldTrimWhitespace()
        {
            var name = CourseName.Create("   Backend   ");

            name.Value.Should().Be("Backend");
        }

        [Fact]
        public void Create_ShouldCollapseMultipleSpaces()
        {
            var name = CourseName.Create("Backend     Development");

            name.Value.Should().Be("Backend Development");
        }

        [Fact]
        public void Create_ShouldConvertToTitleCase()
        {
            var name = CourseName.Create("backend development");

            name.Value.Should().Be("Backend Development");
        }

        [Fact]
        public void Create_ShouldNormalizeMixedCase()
        {
            var name = CourseName.Create("bAcKeNd DeVeLoPmEnT");

            name.Value.Should().Be("Backend Development");
        }

        [Fact]
        public void Create_WithWhitespaceOnly_ShouldThrow()
        {
            var act = () => CourseName.Create("   ");

            act.Should().Throw<InvalidCourseNameException>()
               .WithMessage("A course name is required");
        }

        [Fact]
        public void Create_WithTooLongName_ShouldThrow()
        {
            var text = new string('A', CourseName.MaxLength + 1);

            var act = () => CourseName.Create(text);

            act.Should().Throw<InvalidCourseNameException>()
               .WithMessage($"Course description can not contain more than {CourseName.MaxLength} characters");
        }

        [Fact]
        public void Equality_WithSameNormalizedValue_ShouldBeEqual()
        {
            var n1 = CourseName.Create("backend development");
            var n2 = CourseName.Create("Backend Development");

            n1.Should().Be(n2);
        }

        [Fact]
        public void ToString_ShouldReturnNormalizedValue()
        {
            var name = CourseName.Create("backend development");

            name.ToString().Should().Be("Backend Development");
        }
    }
}
