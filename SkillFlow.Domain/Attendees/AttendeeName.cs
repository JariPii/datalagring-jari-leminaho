using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct AttendeeName
    {
        public string FirstName { get; }
        public string LastName { get; }

        private AttendeeName(string firstName, string lastName)
        {
            FirstName = firstName.Trim();
            LastName = lastName.Trim();            
        }

        public static AttendeeName Create(string firstName, string lastName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));

            return new AttendeeName(firstName, lastName);
        }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
