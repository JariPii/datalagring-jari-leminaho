using FluentAssertions;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;
using Xunit;

namespace SkillFlow.Tests.Domain.Competences
{
    public class CompetenceNameTests
    {
        [Fact]
        public void Create_ShouldTrimWhitespace()
        {
            var name = CompetenceName.Create("   c#   ");

            name.Value.Should().Be("C#");
        }

        [Fact]
        public void Create_ShouldCollapseMultipleSpaces()
        {
            var name = CompetenceName.Create("dotnet     core");

            name.Value.Should().Be("Dotnet Core");
        }

        [Fact]
        public void Create_ShouldConvertToTitleCase()
        {
            var name = CompetenceName.Create("backend development");

            name.Value.Should().Be("Backend Development");
        }

        [Fact]
        public void Create_ShouldNormalizeMixedCase()
        {
            var name = CompetenceName.Create("bAcKeNd dEvElOpMeNt");

            name.Value.Should().Be("Backend Development");
        }

        [Fact]
        public void Create_WithWhitespaceOnly_ShouldThrow()
        {
            var act = () => CompetenceName.Create("   ");

            act.Should().Throw<InvalidCompetenceNameException>()
               .WithMessage("A competence name is required");
        }

        [Fact]
        public void Create_WhenTooLong_ShouldThrow()
        {
            var tooLong = new string('A', CompetenceName.MaxLength + 1);

            var act = () => CompetenceName.Create(tooLong);

            act.Should().Throw<InvalidCompetenceNameException>()
               .WithMessage($"The competence name cannot contain more than {CompetenceName.MaxLength} characters");
        }

        [Fact]
        public void Create_WithMaxLength_ShouldSucceed()
        {
            var valid = new string('A', CompetenceName.MaxLength);

            var name = CompetenceName.Create(valid);

            name.Value.Should().Be(valid.NormalizeName());
            name.Value.Length.Should().Be(CompetenceName.MaxLength);
        }

        [Fact]
        public void Equality_WithSameNormalizedValue_ShouldBeEqual()
        {
            var n1 = CompetenceName.Create("backend development");
            var n2 = CompetenceName.Create("Backend   Development");

            n1.Should().Be(n2);
        }

        [Fact]
        public void ToString_ShouldReturnNormalizedValue()
        {
            var name = CompetenceName.Create("backend development");

            name.ToString().Should().Be("Backend Development");
        }
    }
}
