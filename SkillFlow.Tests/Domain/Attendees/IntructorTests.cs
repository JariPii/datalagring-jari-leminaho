using FluentAssertions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Competences;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Domain.Attendees
{
    public class IntructorTests
    {
        [Fact]
        public void CreateInstructor_ShouldReturnInstructor_AndSetRoleInstructor()
        {
            var instructor = Attendee.CreateInstructor(
                Email.Create("i@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null
            );

            instructor.Should().BeOfType<Instructor>();
            instructor.Role.Should().Be(Role.Instructor);
            instructor.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void AddCompetence_WhenNew_ShouldAdd_AndSetUpdatedAt()
        {
            var instructor = (Instructor)Attendee.CreateInstructor(
                Email.Create("i@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null
            );

            var competence = CreateCompetence();

            instructor.AddCompetence(competence);

            instructor.Competences.Should().ContainSingle();
            instructor.Competences.Should().Contain(c => c.Id == competence.Id);
            instructor.UpdatedAt.Should().NotBeNull();
            instructor.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void AddCompetence_WhenSameInstanceAddedTwice_ShouldNotAddDuplicate()
        {
            var instructor = (Instructor)Attendee.CreateInstructor(
                Email.Create("i@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null
            );

            var competence = CreateCompetence();

            instructor.AddCompetence(competence);
            instructor.AddCompetence(competence);

            instructor.Competences.Should().HaveCount(1);
        }

        [Fact]
        public void AddCompetence_WhenDuplicate_ShouldNotChangeUpdatedAt()
        {
            var instructor = (Instructor)Attendee.CreateInstructor(
                Email.Create("i@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null
            );

            var competence = CreateCompetence();

            instructor.AddCompetence(competence);
            var t1 = instructor.UpdatedAt;

            instructor.AddCompetence(competence);
            var t2 = instructor.UpdatedAt;

            t2.Should().Be(t1);
        }

        private static Competence CreateCompetence(string name = "C#")
            => Competence.Create(CompetenceName.Create(name));
    }
}
