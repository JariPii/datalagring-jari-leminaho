using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Attendees
{
    public readonly record struct Email(string Value)
    {

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email can not be empty", nameof(value));

            if (!value.Contains("@"))
                throw new ArgumentException("Invalid email format", nameof(value));

            return new Email(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
    }
}
