using FluentAssertions;
using SkillFlow.Domain.Entities.Competences;
using Xunit;

namespace SkillFlow.Tests.Domain.Competences
{
    public class CompetenceIdTests
    {
        [Fact]
        public void Constructor_WithValidGuid_ShouldCreateId()
        {
            var guid = Guid.NewGuid();

            var id = new CompetenceId(guid);

            id.Value.Should().Be(guid);
        }

        [Fact]
        public void Constructor_WithEmptyGuid_ShouldThrowException()
        {
            var act = () => new CompetenceId(Guid.Empty);

            act.Should().Throw<ArgumentException>()
               .WithMessage("Competence Id can not be empty*");
        }

        [Fact]
        public void New_ShouldCreateValidNonEmptyId()
        {
            var id = CompetenceId.New();

            id.Value.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void New_ShouldCreateUniqueIds()
        {
            var id1 = CompetenceId.New();
            var id2 = CompetenceId.New();

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void Equality_WithSameGuid_ShouldBeEqual()
        {
            var guid = Guid.NewGuid();

            var id1 = new CompetenceId(guid);
            var id2 = new CompetenceId(guid);

            id1.Should().Be(id2);
        }

        [Fact]
        public void Equality_WithDifferentGuid_ShouldNotBeEqual()
        {
            var id1 = new CompetenceId(Guid.NewGuid());
            var id2 = new CompetenceId(Guid.NewGuid());

            id1.Should().NotBe(id2);
        }

        [Fact]
        public void ToString_ShouldReturnGuidString()
        {
            var guid = Guid.NewGuid();
            var id = new CompetenceId(guid);

            id.ToString().Should().Be(guid.ToString());
        }
    }
}

