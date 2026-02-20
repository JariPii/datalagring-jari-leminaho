using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Primitives;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Courses
{
    public readonly partial record struct CourseName
    {
        public const int MaxLength = 50;
        public string Value { get; }

        private CourseName(string value) => Value = value;
        public static CourseName Create(string value)
        {

            var cleanCourseName = value.NormalizeName();

            if (cleanCourseName.Length == 0)
                throw new InvalidCourseNameException("A course name is required");

            if (cleanCourseName.Length > MaxLength)
                throw new InvalidCourseNameException($"Course description can not contain more than {MaxLength} characters");

            return new CourseName(cleanCourseName);
        }

        public override string ToString() => Value;
    }
}
