using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{
    public readonly record struct CourseDescription
    {
        public string Value { get; }
        public const int MaxLength = 1000;

        private CourseDescription(string value)
        {
            Value = value;
        }

        public static CourseDescription Create(string value)
        {
            value ??= string.Empty;

            var trimmedValue = value.Trim();

            if (trimmedValue.Length > MaxLength)
                throw new ArgumentException("Too long description", nameof(value));


            return new CourseDescription(trimmedValue);
        }

        public override string ToString() => Value;
    }
}
