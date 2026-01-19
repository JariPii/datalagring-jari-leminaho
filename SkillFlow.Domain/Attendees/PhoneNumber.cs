using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct PhoneNumber
    {
        public string Value { get; }
        private PhoneNumber(string value) => Value = value;
        public static PhoneNumber? Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (value.Length < 8)
                throw new ArgumentException("Invalid phone number", nameof(value));

            return new PhoneNumber(value);
        }

        public override string ToString() => Value;
    }
}
