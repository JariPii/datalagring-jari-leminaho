using FluentAssertions;
using SkillFlow.Domain.Entities.Courses;
using Xunit;

namespace SkillFlow.Tests.Domain.Courses
{
    public class CourseIdTests
    {
        [Fact]
        public void Constructor_WithValidGuid_ShouldCreateId()
        {
            var guid = Guid.NewGuid();

            var id = new CourseId(guid);

            id.Value.Should().Be(guid);
        }

        [Fact]
        public void Constructor_WithEmptyGuid_ShouldThrowException()
        {
            var act = () => new CourseId(Guid.Empty);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Course id can not be empty*");
        }

        [Fact]
        public void New_ShouldCreateValidNonEmptyId()
        {
            var id = CourseId.New();

            id.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void New_ShouldCreateUniqueIds()
        {
            var id1 = CourseId.New();
            var id2 = CourseId.New();

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void Equality_WithSameGuid_ShouldBeEqual()
        {
            var guid = Guid.NewGuid();

            var id1 = new CourseId(guid);
            var id2 = new CourseId(guid);

            id1.Should().Be(id2);
        }

        [Fact]
        public void Equality_WithDifferentGuid_ShouldNotBeEqual()
        {
            var id1 = new CourseId(Guid.NewGuid());
            var id2 = new CourseId(Guid.NewGuid());

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void ToString_ShouldReturnGuidString()
        {
            var guid = Guid.NewGuid();
            var id = new CourseId(guid);

            id.ToString().Should().Be(guid.ToString());
        }
    }
}
