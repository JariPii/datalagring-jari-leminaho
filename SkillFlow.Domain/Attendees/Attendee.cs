using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public abstract class Attendee : BaseEntity
    {

        public AttendeeId Id { get; private set; } = null!;
        public Role Role { get; private set; }

        
        public string Email { get; private set; } = null!;
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }

        protected Attendee(AttendeeId id, string email, string firstName, string lastName, Role role, string phoneNumber = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("E-mail is required", nameof(email));

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("E-mail is required", nameof(lastName));

            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            PhoneNumber = phoneNumber;
        }

        protected Attendee() { }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail) || !newEmail.Contains("@"))
                throw new ArgumentException("Invalid email format");

            if (Email == newEmail) return;

            Email = newEmail;
            UpdateTimeStamp();
        }

        public void UpdateFirstName(string newFirstName)
        {
            if (string.IsNullOrWhiteSpace(newFirstName))
                throw new ArgumentException("Firstname is required");

            if (FirstName == newFirstName) return;

            FirstName = newFirstName;
            UpdateTimeStamp();
        }

        public void UpdateLastName(string newLastName)
        {
            if (string.IsNullOrWhiteSpace(newLastName))
                throw new ArgumentException("Firstname is required");

            if (LastName == newLastName) return;

            LastName = newLastName;
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
