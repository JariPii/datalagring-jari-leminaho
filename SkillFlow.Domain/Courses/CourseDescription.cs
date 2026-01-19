using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CourseDescription
    {
        public string Value { get; }
        public const int MaxLength = 1000;

        public CourseDescription(string value)
        {
            value ??= string.Empty;

            if (value.Length > MaxLength)
                throw new ArgumentException("Too long description", nameof(value));

            Value = value.Trim();
        }

        public override string ToString() => Value;
    }
}
