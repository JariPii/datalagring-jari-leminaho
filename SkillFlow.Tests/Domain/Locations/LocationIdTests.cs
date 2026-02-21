using FluentAssertions;
using SkillFlow.Domain.Entities.Locations;
using Xunit;

namespace SkillFlow.Tests.Domain.Locations
{
    public class LocationIdTests
    {
        [Fact]
        public void Constructor_WithValidGuid_ShouldCreateId()
        {
            var guid = Guid.NewGuid();

            var id = new LocationId(guid);

            id.Value.Should().Be(guid);
        }

        [Fact]
        public void Constructor_WithEmptyGuid_ShouldThrowException()
        {
            var act = () => new LocationId(Guid.Empty);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Location Id can not be empty (Parameter 'value')");
        }

        [Fact]
        public void New_ShouldCreateValidNonEmptyId()
        {
            var id = LocationId.New();

            id.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void New_ShouldCreateUniqueIds()
        {
            var id1 = LocationId.New();
            var id2 = LocationId.New();

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void Equality_WithSameGuid_ShouldBeEqual()
        {
            var guid = Guid.NewGuid();

            var id1 = new LocationId(guid);
            var id2 = new LocationId(guid);

            id1.Should().Be(id2);
        }

        [Fact]
        public void Equality_WithDifferentGuid_ShouldNotBeEqual()
        {
            var id1 = new LocationId(Guid.NewGuid());
            var id2 = new LocationId(Guid.NewGuid());

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void ToString_ShouldReturnGuidString()
        {
            var guid = Guid.NewGuid();
            var id = new LocationId(guid);

            id.ToString().Should().Be(guid.ToString());
        }
    }
}
