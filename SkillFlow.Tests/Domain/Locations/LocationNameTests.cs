using FluentAssertions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;
using Xunit;

namespace SkillFlow.Tests.Domain.Locations
{
    public class LocationNameTests
    {
        [Fact]
        public void Create_ShouldTrimAndNormalizeToTitleCase()
        {
            var name = LocationName.Create("   göteborg   ");

            name.Value.Should().Be("Göteborg");
            name.ToString().Should().Be("Göteborg");
        }

        [Fact]
        public void Create_ShouldCollapseMultipleSpaces()
        {
            var name = LocationName.Create("New     York");

            name.Value.Should().Be("New York");
        }

        [Fact]
        public void Create_WithWhitespaceOnly_ShouldThrow()
        {
            var act = () => LocationName.Create("   ");

            act.Should().Throw<InvalidLocationNameException>()
               .WithMessage("*Location name is required*");
        }

        [Fact]
        public void Create_WhenTooLong_ShouldThrow()
        {
            var input = new string('A', LocationName.MaxLength + 1);

            var act = () => LocationName.Create(input);

            act.Should().Throw<InvalidLocationNameException>()
               .WithMessage($"*Location name cannot exceed {LocationName.MaxLength} characters*");
        }

        [Fact]
        public void Create_WithMaxLength_ShouldSucceed()
        {
            var input = new string('A', LocationName.MaxLength);

            var name = LocationName.Create(input);

            name.Value.Should().Be(input.NormalizeName());
            name.Value.Length.Should().Be(LocationName.MaxLength);
        }

        [Fact]
        public void Equality_WithSameNormalizedValue_ShouldBeEqual()
        {
            var n1 = LocationName.Create("new   york");
            var n2 = LocationName.Create("New York");

            n1.Should().Be(n2);
        }
    }
}
