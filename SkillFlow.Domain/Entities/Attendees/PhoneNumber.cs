using SkillFlow.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Entities.Attendees
{
    public readonly partial record struct PhoneNumber
    {
        public string Value { get; }
        private PhoneNumber(string value) => Value = value;


        public static PhoneNumber? Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var trimmed = MyRegex().Replace(value.Trim(), "");

            if (!PhoneNumberRegex().IsMatch(trimmed))
                throw new InvalidPhoneNumberException("Invalid phone number format, e.g. +46123321456");

            return new PhoneNumber(trimmed);
        }

        [GeneratedRegex(@"^\+[1-9]\d{7,14}$")]
        private static partial Regex PhoneNumberRegex();

        [GeneratedRegex(@"[\s\-\.\(\)]")]
        private static partial Regex MyRegex();

        public override string ToString() => Value;
    }
}
