using SkillFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Attendees
{
    public readonly partial record struct AttendeeName
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
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new InvalidNameException($"First name and last name is required");

            var generateCleanFirstName = MyRegex().Replace(firstName.Trim(), " ");
            var generatedCleanLastName = MyRegex().Replace(lastName.Trim(), " ");

            if (generateCleanFirstName.Length > MaxLength || generatedCleanLastName.Length > MaxLength)
                throw new InvalidNameException($"Name can not exceed {MaxLength} characters");

            return new AttendeeName(generateCleanFirstName, generatedCleanLastName);
        }

        public override string ToString() => $"{FirstName} {LastName}";

        [GeneratedRegex(@"\s+")]
        private static partial Regex MyRegex();
    }
}
