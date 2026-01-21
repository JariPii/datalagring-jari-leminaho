using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CourseName
    {
        public const int MaxLength = 200;
        public string Value { get; }

        private CourseName(string value)
        {
            Value = value;
        }

        public static CourseName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

            var trimmedValue = value.Trim();

            if (trimmedValue.Length > MaxLength)
                throw new ArgumentException($"The course name can not exceed {MaxLength} characters", nameof(value));

            return new CourseName(trimmedValue);
        }

        public override string ToString() => Value;
    }
}
