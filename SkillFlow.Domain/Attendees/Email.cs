using SkillFlow.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct Email
    {
        public string Value { get; }

        private Email(string value) => Value = value;

        public static Email Create(string value)
        {

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidEmailException($"Email is required");

            if (!value.Contains("@"))
                throw new InvalidEmailException($"Invalid email format");

            return new Email(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
    }
}
