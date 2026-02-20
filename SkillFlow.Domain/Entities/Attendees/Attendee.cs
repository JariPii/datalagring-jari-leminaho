using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Attendees
{
    public abstract class Attendee : BaseEntity<AttendeeId>, IAggregateRoot
    {
        public Role Role { get; private init; }
        public Email Email { get; private set; }

        public string EmailUnique { get; private set; } = default!;
        public AttendeeName Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }

        public static Student CreateStudent(Email email, AttendeeName name, PhoneNumber? phoneNumber)
            => new(AttendeeId.New(), email, name, phoneNumber);

        public static Instructor CreateInstructor(Email email, AttendeeName name, PhoneNumber? phoneNumber)
            => new(AttendeeId.New(), email, name, phoneNumber);

        protected Attendee(AttendeeId id, Email email, AttendeeName name, PhoneNumber? phoneNumber, Role role)
        {
            Id = id;
            Email = email;
            EmailUnique = email.UniqueValue;
            Name = name;
            PhoneNumber = phoneNumber;
            Role = role;
        }

        protected Attendee() { }

        public void UpdateEmail(Email newEmail)
        {

            if (Email == newEmail) return;

            Email = newEmail;
            EmailUnique = newEmail.UniqueValue;
            UpdateTimeStamp();
        }

        public void UpdateName(AttendeeName newName)
        {
            if (Name == newName) return;
            Name = newName;
            UpdateTimeStamp();
        }

        public void UpdateFirstName(string newFirstName)
            => UpdateName(AttendeeName.Create(newFirstName, Name.LastName));

        public void UpdateLastName(string newLastName) 
            => UpdateName(AttendeeName.Create(Name.FirstName, newLastName));

        public void UpdatePhoneNumber(PhoneNumber? newPhoneNumber)
        {
            if (PhoneNumber == newPhoneNumber) return;

            PhoneNumber = newPhoneNumber;
            UpdateTimeStamp();
        }

    }
}
