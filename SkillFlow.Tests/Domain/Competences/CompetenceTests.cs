using FluentAssertions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using Xunit;

namespace SkillFlow.Tests.Domain.Competences
{
    public class CompetenceTests
    {
        [Fact]
        public void Create_ShouldSetName_AndLeaveUpdatedAtNull()
        {
            var name = CompetenceName.Create("C#");

            var competence = Competence.Create(name);

            competence.Id.Value.Should().NotBe(Guid.Empty);
            competence.Name.Should().Be(name);
            competence.UpdatedAt.Should().BeNull();
            competence.Instructors.Should().BeEmpty();
        }

        [Fact]
        public void UpdateCompetenceName_WhenDifferent_ShouldUpdateName_AndSetUpdatedAt()
        {
            var competence = Competence.Create(CompetenceName.Create("C#"));
            competence.UpdatedAt.Should().BeNull();

            var newName = CompetenceName.Create("Dotnet");

            competence.UpdateCompetenceName(newName);

            competence.Name.Should().Be(newName);
            competence.UpdatedAt.Should().NotBeNull();
            competence.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void UpdateCompetenceName_WhenSame_ShouldNotChangeUpdatedAt()
        {
            var competence = Competence.Create(CompetenceName.Create("C#"));

            competence.UpdateCompetenceName(CompetenceName.Create("Dotnet"));
            var t1 = competence.UpdatedAt;

            competence.UpdateCompetenceName(CompetenceName.Create("Dotnet"));

            competence.UpdatedAt.Should().Be(t1);
        }

        [Fact]
        public void AddInstructor_WhenNull_ShouldThrow()
        {
            var competence = Competence.Create(CompetenceName.Create("C#"));

            var act = () => competence.AddInstructor(null!);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddInstructor_WhenNew_ShouldAdd_AndSetUpdatedAt()
        {
            var competence = Competence.Create(CompetenceName.Create("C#"));
            var instructor = CreateInstructor();

            competence.AddInstructor(instructor);

            competence.Instructors.Should().ContainSingle();
            competence.Instructors.Should().Contain(i => i.Id == instructor.Id);
            competence.UpdatedAt.Should().NotBeNull();
            competence.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void AddInstructor_WhenDuplicateById_ShouldNotAdd()
        {
            var competence = Competence.Create(CompetenceName.Create("C#"));

            var instructor = CreateInstructor();

            competence.AddInstructor(instructor);
            competence.AddInstructor(instructor);

            competence.Instructors.Should().HaveCount(1);
        }

        [Fact]
        public void AddInstructor_WhenDuplicate_ShouldNotChangeUpdatedAt()
        {
            var competence = Competence.Create(CompetenceName.Create("C#"));

            var instructor = CreateInstructor();

            competence.AddInstructor(instructor);
            var t1 = competence.UpdatedAt;

            competence.AddInstructor(instructor);
            var t2 = competence.UpdatedAt;

            t2.Should().Be(t1);
        }

        private static Instructor CreateInstructor()
            => (Instructor)Attendee.CreateInstructor(
                Email.Create("i@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null
            );
    }
}
