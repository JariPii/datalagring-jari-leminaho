using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public abstract class Attendee : BaseEntity
    {

        public AttendeeId Id { get; private set; }
        public Role Role { get; private set; }

        
        public Email Email { get; private set; }
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }

        protected Attendee(AttendeeId id, Email email, string firstName, string lastName, Role role, string? phoneNumber = null)
        {
            if (id.Value == Guid.Empty)
                throw new ArgumentException("Attendee Id can not be empty", nameof(id));

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required", nameof(lastName));

            Id = id;
            Email = email;
            FirstName = firstName.NormalizeName();
            LastName = lastName.NormalizeName();
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

        public void UpdateFirstName(string newFirstName)
        {
            if (string.IsNullOrWhiteSpace(newFirstName))
                throw new ArgumentException("Firstname is required");

            var normalizedName = newFirstName.NormalizeName();

            if (FirstName == normalizedName) return;

            FirstName = normalizedName;
            UpdateTimeStamp();
        }

        public void UpdateLastName(string newLastName)
        {
            if (string.IsNullOrWhiteSpace(newLastName))
                throw new ArgumentException("Lastname is required");

            var normalizedName = newLastName.NormalizeName();

            if (LastName == normalizedName) return;

            LastName = normalizedName;
            UpdateTimeStamp();
        }

        public void UpdatePhoneNumber(string? newPhoneNumber)
        {
            if (PhoneNumber == newPhoneNumber) return;

            PhoneNumber = newPhoneNumber;
            UpdateTimeStamp();
        }

    }
}
