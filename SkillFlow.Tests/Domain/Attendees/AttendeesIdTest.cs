using FluentAssertions;
using SkillFlow.Domain.Entities.Attendees;
using Xunit;

namespace SkillFlow.Tests.Domain.Attendees
{
    public class AttendeesIdTest
    {
        [Fact]
        public void Constructor_WithValidGuid_ShouldCreateId()
        {
            var guid = Guid.NewGuid();

            var id = new AttendeeId(guid);

            id.Value.Should().Be(guid);
        }

        [Fact]
        public void Constructor_WithEmptyGuid_ShouldThrowException()
        {
            var act = () => new AttendeeId(Guid.Empty);

            act.Should().Throw<ArgumentException>()
               .WithMessage("AttendeeId can not be empty*");
        }

        [Fact]
        public void New_ShouldCreateValidNonEmptyId()
        {
            var id = AttendeeId.New();

            id.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void New_ShouldCreateUniqueIds()
        {
            var id1 = AttendeeId.New();
            var id2 = AttendeeId.New();

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void Equality_WithSameGuid_ShouldBeEqual()
        {
            var guid = Guid.NewGuid();

            var id1 = new AttendeeId(guid);
            var id2 = new AttendeeId(guid);

            id1.Should().Be(id2);
        }

        [Fact]
        public void Equality_WithDifferentGuid_ShouldNotBeEqual()
        {
            var id1 = new AttendeeId(Guid.NewGuid());
            var id2 = new AttendeeId(Guid.NewGuid());

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void ToString_ShouldReturnGuidString()
        {
            var guid = Guid.NewGuid();
            var id = new AttendeeId(guid);

            id.ToString().Should().Be(guid.ToString());
        }
    }
}
