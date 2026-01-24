using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Courses
{
    public readonly partial record struct CourseDescription
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
            var generatedCleanCourseDescription = MyRegex().Replace(value.Trim(), " ");

            if (generatedCleanCourseDescription.Length > MaxLength)
                throw new ArgumentException("Too long description", nameof(value));


            return new CourseDescription(generatedCleanCourseDescription);
        }

        public override string ToString() => Value;

        [GeneratedRegex(@"\s+")]
        private static partial Regex MyRegex();
    }
}
