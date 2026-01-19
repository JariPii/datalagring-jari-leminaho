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
            
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            if (!value.Contains("@"))
                throw new ArgumentException("Invalid email format", nameof(value));

            return new Email(value.Trim().ToLowerInvariant());
        }

        public override string ToString() => Value;
    }
}
