using SkillFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct PhoneNumber
    {
        public string Value { get; }

        public const int MaxLength = 8;
        private PhoneNumber(string value) => Value = value;
        public static PhoneNumber? Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (value.Length < MaxLength)
                throw new InvalidPhoneNumberException($"Name can not exceed {MaxLength} characters");

            return new PhoneNumber(value);
        }

        public override string ToString() => Value;
    }
}
