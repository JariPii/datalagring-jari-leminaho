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
            var cleanFirstName = firstName.NormalizeName();
            var cleanLastName = lastName.NormalizeName();

            if (cleanFirstName.Length > MaxLength || cleanLastName.Length > MaxLength)
                throw new InvalidNameException($"Name can not exceed {MaxLength} characters");

            if (!NameRegex().IsMatch(cleanFirstName) || !NameRegex().IsMatch(cleanLastName))
                throw new InvalidNameException("Name contains invalid charachters");

            return new AttendeeName(cleanFirstName, cleanLastName);
        }

        [GeneratedRegex(@"^[\p{L}]+(?:[ '-][\p{L}]+)*$")]
        private static partial Regex NameRegex();

        public string Fullname => $"{FirstName} {LastName}";
        public override string ToString() => Fullname;
    }
}
