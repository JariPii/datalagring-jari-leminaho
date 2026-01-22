using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CompetenceName
    {
        public const int MaxLength = 200;
        public string Value { get; }

        private CompetenceName(string value) => Value = value;

        public static CompetenceName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            var trimmedValue = value.Trim();

            if (trimmedValue.Length > MaxLength)
                throw new ArgumentException($"The competence name cannot contain more than {MaxLength} characters", nameof(value));

            return new CompetenceName(trimmedValue);

        }

        public override string ToString() => Value;
    }
}
