using SkillFlow.Domain.Primitives;
using System.Text.RegularExpressions;

namespace SkillFlow.Domain.Courses
{
    public readonly partial record struct CourseDescription
    {
        public const int MaxLength = 1000;
        public string Value { get; }

        private CourseDescription(string value)
        {
            Value = value;
        }

        public static CourseDescription Create(string value)
        {
            var cleanValue = value.NormalizeText();

            if (cleanValue.Length > MaxLength)
                throw new ArgumentException("Too long description", nameof(value));

            return new CourseDescription(cleanValue);
        }

        public override string ToString() => Value;
    }
}
