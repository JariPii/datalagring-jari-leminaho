using FluentAssertions;
using SkillFlow.Domain.Courses;
using Xunit;

namespace SkillFlow.Tests.Domain.Courses
{
    public class CourseDescriptionTests
    {
        [Fact]
        public void Create_WithValidValue_ShouldCreateDescription()
        {
            var desc = CourseDescription.Create("Intro to backend");

            desc.Value.Should().Be("Intro to backend");
            desc.ToString().Should().Be("Intro to backend");
        }

        [Fact]
        public void Create_ShouldNormalizeText()
        {
            var desc = CourseDescription.Create("   Intro to backend   ");

            // Anpassa detta beroende på vad NormalizeText() gör.
            // Vanligast är Trim().
            desc.Value.Should().Be("Intro to backend");
        }

        [Fact]
        public void Create_WithMaxLength_ShouldSucceed()
        {
            var text = new string('A', CourseDescription.MaxLength);

            var desc = CourseDescription.Create(text);

            desc.Value.Should().Be(text);
        }

        [Fact]
        public void Create_WhenTooLong_ShouldThrow()
        {
            var text = new string('A', CourseDescription.MaxLength + 1);

            var act = () => CourseDescription.Create(text);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Too long description*")
               .And.ParamName.Should().Be("value");
        }

        [Fact]
        public void Equality_WithSameValue_ShouldBeEqual()
        {
            var d1 = CourseDescription.Create("Backend");
            var d2 = CourseDescription.Create("Backend");

            d1.Should().Be(d2);
        }

        [Fact]
        public void Equality_WithDifferentValue_ShouldNotBeEqual()
        {
            var d1 = CourseDescription.Create("Backend");
            var d2 = CourseDescription.Create("Frontend");

            d1.Should().NotBe(d2);
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            var desc = CourseDescription.Create("Backend");

            desc.ToString().Should().Be("Backend");
        }

        [Fact]
        public void Create_ShouldCollapseWhitespace()
        {
            var desc = CourseDescription.Create("Hello   World");

            desc.Value.Should().Be("Hello World");
        }
    }
}
