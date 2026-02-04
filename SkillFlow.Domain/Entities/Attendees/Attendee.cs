using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Attendees
{
    public abstract class Attendee : BaseEntity<AttendeeId>, IAggregateRoot
    {
        public Role Role { get; private set; }
        public Email Email { get; private set; }
        public AttendeeName Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }

        public static Attendee Create(Email email, AttendeeName name, Role role, PhoneNumber? phoneNumber)
        {
            var id = AttendeeId.New();

            return role switch
            {
                Role.Instructor => new Instructor(id, email, name, phoneNumber),
                Role.Student => new Student(id, email, name, phoneNumber),
                _ => throw new InvalidRoleException(role)
            };
        }


        protected Attendee(AttendeeId id, Email email, AttendeeName name, Role role, PhoneNumber? phoneNumber)
        {
            Id = id;
            Email = email;
            Name = name;
            Role = role;
            PhoneNumber = phoneNumber;
        }

        protected Attendee() { }

        public void UpdateEmail(Email newEmail)
        {

            if (Email == newEmail) return;

            Email = newEmail;
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
