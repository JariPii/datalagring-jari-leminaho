using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Attendees
{
    public readonly partial record struct AttendeeName
    {
        public const int MaxLength = 50;
        public string FirstName { get; }
        public string LastName { get; }

        private AttendeeName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;            
        }

        public static AttendeeName Create(string firstName, string lastName)
        {
            //if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            //    throw new InvalidNameException($"First name and last name is required");

            var cleanFirstName = firstName.NormalizeName();
            var cleanLastName = lastName.NormalizeName();

            if (cleanFirstName.Length > MaxLength || cleanLastName.Length > MaxLength)
                throw new InvalidNameException($"Name can not exceed {MaxLength} characters");

            return new AttendeeName(cleanFirstName, cleanLastName);
        }

        public string Fullname => $"{FirstName} {LastName}";
        public override string ToString() => Fullname;
    }
}
