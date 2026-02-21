using FluentAssertions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using Xunit;

namespace SkillFlow.Tests.Domain.Attendees
{
    public class AttendeesTests
    {
        [Fact]
        public void CreateStudent_ShouldSetEmailUnique_FromEmail()
        {
            var email = Email.Create("A.Test@Example.com");
            var name = AttendeeName.Create("Jari", "P");

            var student = Attendee.CreateStudent(email, name, null);

            student.Email.Should().Be(email);
            student.EmailUnique.Should().Be(email.UniqueValue);
            student.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void UpdateEmail_WhenDifferent_ShouldUpdateEmail_AndEmailUnique_AndUpdatedAt()
        {
            var attendee = Attendee.CreateStudent(
                Email.Create("a@b.com"),
                AttendeeName.Create("Jari", "P"),
                null
            );

            var newEmail = Email.Create("New@B.com");

            attendee.UpdateEmail(newEmail);

            attendee.Email.Should().Be(newEmail);
            attendee.EmailUnique.Should().Be(newEmail.UniqueValue);
            attendee.UpdatedAt.Should().NotBeNull();
            attendee.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public void UpdateEmail_WhenSame_ShouldNotChangeUpdatedAt_OrEmailUnique()
        {
            var email = Email.Create("a@b.com");
            var attendee = Attendee.CreateStudent(
                email,
                AttendeeName.Create("Jari", "P"),
                null
            );

            attendee.UpdateEmail(Email.Create("x@y.com"));
            var t1 = attendee.UpdatedAt;
            var emailUnique1 = attendee.EmailUnique;

            attendee.UpdateEmail(Email.Create("x@y.com"));

            attendee.UpdatedAt.Should().Be(t1);
            attendee.EmailUnique.Should().Be(emailUnique1);
        }

        [Fact]
        public void UpdateFirstName_ShouldKeepLastName_AndUpdateTimestamp()
        {
            var attendee = Attendee.CreateStudent(
                Email.Create("a@b.com"),
                AttendeeName.Create("Jari", "P"),
                null
            );

            attendee.UpdateFirstName("NewFirst");

            var expected = AttendeeName.Create("NewFirst", "P");

            attendee.Name.Should().Be(expected);
            attendee.Name.LastName.Should().Be("P");
            attendee.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdatePhoneNumber_WhenSame_ShouldNotTouchUpdatedAt()
        {
            var phone = PhoneNumber.Create("+46701234567");

            var attendee = Attendee.CreateStudent(
                Email.Create("a@b.com"),
                AttendeeName.Create("Jari", "P"),
                phone
            );

            attendee.UpdateName(AttendeeName.Create("X", "Y"));
            var t1 = attendee.UpdatedAt;

            attendee.UpdatePhoneNumber(PhoneNumber.Create("+46701234567"));

            attendee.UpdatedAt.Should().Be(t1);
        }
    }
}
