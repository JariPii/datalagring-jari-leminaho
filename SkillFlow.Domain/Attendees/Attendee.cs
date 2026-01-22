using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Attendees
{
    public abstract class Attendee : BaseEntity
    {
        public AttendeeId Id { get; private set; }
        public Role Role { get; private set; }
        public Email Email { get; private set; }
        public AttendeeName Name { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }


        protected Attendee(AttendeeId id, Email email, AttendeeName name, Role role, PhoneNumber? phoneNumber)
        {
            Id = id;
            Email = email;
            Name = name;
            Role = role;
            PhoneNumber = phoneNumber;
        }

        protected Attendee() { }

        public void UpdateEmail(string newUpdatedEmail)
        {
            var newEmail = Email.Create(newUpdatedEmail);

            if (Email == newEmail) return;

            Email = newEmail;
            UpdateTimeStamp();
        }

        public void UpdateFirstName(string newFirstName)
        {
            var newName = AttendeeName.Create(newFirstName, Name.LastName);

            if (Name == newName) return;

            Name = newName;
            UpdateTimeStamp();
        }
        public void UpdateLastName(string newLastName)
        {
            var newName = AttendeeName.Create(Name.FirstName, newLastName);

            if (Name == newName) return;

            Name = newName;
            UpdateTimeStamp();
        }

        public void UpdatePhoneNumber(PhoneNumber? newPhoneNumber)
        {
            if (PhoneNumber == newPhoneNumber) return;

            PhoneNumber = newPhoneNumber;
            UpdateTimeStamp();
        }

    }
}
