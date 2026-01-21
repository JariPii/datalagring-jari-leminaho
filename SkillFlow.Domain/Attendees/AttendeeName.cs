using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct AttendeeName
    {
        public const int MaxLength = 150;
        public string FirstName { get; }
        public string LastName { get; }

        private AttendeeName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;            
        }

        public static AttendeeName Create(string firstName, string lastName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

            var trimmedFirstName = firstName.Trim();
            var trimmedLastName = lastName.Trim();

            if (trimmedFirstName.Length > MaxLength)
                throw new ArgumentException($"First name can only hold {MaxLength} characters", nameof(firstName));

            if (trimmedLastName.Length > MaxLength)
                throw new ArgumentException($"Last name can only hold {MaxLength} characters", nameof(lastName));

            return new AttendeeName(trimmedFirstName, trimmedLastName);
        }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
