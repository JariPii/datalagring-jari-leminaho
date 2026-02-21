using FluentAssertions;
using SkillFlow.Domain.Entities.Locations;
using Xunit;

namespace SkillFlow.Tests.Domain.Locations
{
    public class LocationTests
    {
        [Fact]
        public void Create_ShouldSetName_AndLeaveUpdatedAtNull()
        {
            var name = LocationName.Create("Stockholm");

            var location = Location.Create(name);

            location.Id.Should().NotBe(default);
            location.LocationName.Should().Be(name);
            location.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void UpdateLocationName_WhenDifferent_ShouldUpdate_AndSetUpdatedAt()
        {
            var location = Location.Create(LocationName.Create("Stockholm"));
            location.UpdatedAt.Should().BeNull();

            var newName = LocationName.Create("Göteborg");

            location.UpdateLocationName(newName);

            location.LocationName.Should().Be(newName);
            location.UpdatedAt.Should().NotBeNull();
            location.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void UpdateLocationName_WhenSame_ShouldNotChangeUpdatedAt()
        {
            var location = Location.Create(LocationName.Create("Stockholm"));

            location.UpdateLocationName(LocationName.Create("Göteborg"));
            var t1 = location.UpdatedAt;

            location.UpdateLocationName(LocationName.Create("Göteborg"));

            location.UpdatedAt.Should().Be(t1);
        }
    }
}
