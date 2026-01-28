using SkillFlow.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Courses
{
    public readonly partial record struct CourseName
    {
        public const int MaxLength = 200;
        public string Value { get; }

        private CourseName(string value) => Value = value;
        public static CourseName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidCourseNameException("Course name is required");

            var genereatedCleanCourseName = MyRegex().Replace(value.Trim(), " ");

            if (genereatedCleanCourseName.Length > MaxLength)
                throw new InvalidCourseNameException($"Course description can not contain more than {MaxLength} characters");

            return new CourseName(genereatedCleanCourseName);
        }

        public override string ToString() => Value;

        [GeneratedRegex(@"\s+")]
        private static partial Regex MyRegex();
    }
}
