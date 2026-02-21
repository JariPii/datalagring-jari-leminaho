using FluentAssertions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Domain.Attendees
{
    public class StudentTests
    {
        [Fact]
        public void CreateStudent_ShouldReturnStudent_AndSetRoleStudent()
        {
            var student = Attendee.CreateStudent(
                Email.Create("s@test.com"),
                AttendeeName.Create("Jari", "P"),
                null
            );

            student.Should().BeOfType<Student>();
            student.Role.Should().Be(Role.Student);
        }

        [Fact]
        public void CreateStudent_ShouldSetCreatedAt_AndLeaveUpdatedAtNull()
        {
            var student = Attendee.CreateStudent(
                Email.Create("s@test.com"),
                AttendeeName.Create("Jari", "P"),
                null
            );

            student.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            student.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void CreateStudent_ShouldSetEmailUnique_FromEmail()
        {
            var email = Email.Create("S@Test.com");

            var student = Attendee.CreateStudent(
                email,
                AttendeeName.Create("Jari", "P"),
                null
            );

            student.Email.Should().Be(email);
            student.EmailUnique.Should().Be(email.UniqueValue);
        }
    }
}
