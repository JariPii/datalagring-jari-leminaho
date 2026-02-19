using SkillFlow.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace SkillFlow.Domain.Entities.Courses
{

    public readonly record struct CourseCode
    {
        public string Value => $"{CoursePart}{CourseType}-{CourseSuffix:D3}";
        public required string CoursePart { get; init; }
        public required CourseType CourseType { get; init; }
        public required int CourseSuffix { get; init; }

        [SetsRequiredMembers]
        private CourseCode ( string course, CourseType type, int suffix) : this()
        {
            CoursePart = course;
            CourseType = type;
            CourseSuffix = suffix;
        }


        public static CourseCode Create(string coursePartialName, CourseType type, int suffix = 10)
        {
            var part = GetPart(coursePartialName);

            return new CourseCode(part, type, suffix);            
        }

        public static CourseCode CreateUnique(
            string coursePartilName,
            CourseType type,
            Func<string, bool> exists,
            int startSuffix = 10)
        {
            ArgumentNullException.ThrowIfNull(exists);

            _ = Create(coursePartilName, type, startSuffix);

            var part = GetPart(coursePartilName);

            var suffix = startSuffix;

            while (exists(BuildValue(part, type, suffix)))
            {
                suffix += 10;

                if (suffix > 990)
                    throw new InvalidOperationException("No available course codes remaining");
            }

            return new CourseCode(part, type, suffix);
            
        }

        public static CourseCode FromValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 10 || value[6] != '-')
                throw new ArgumentException("Invalid course code format. Expected e.g. MTHGRD-010", nameof(value));

            var part = value[..3];
            var typeStr = value.Substring(3, 3);
            var suffixStr = value.Substring(7, 3);

            if (!Enum.TryParse<CourseType>(typeStr, true, out var type) || !int.TryParse(suffixStr, out var suffix))
                throw new ArgumentException("Invalid course type in course code", nameof(value));

            return new CourseCode(part.ToUpperInvariant(), type, suffix);
        }

        private static string GetPart(string name)
        {
            var clearName = name.Trim().ToUpperInvariant();

            var result = clearName[0].ToString();

            char[] vowels = ['A', 'E', 'I', 'O', 'U', 'Y', 'Å', 'Ä', 'Ö'];

            char[] allowedSpecial = ['#', '+'];

            foreach (var c in clearName.Skip(1))
            {
                if (result.Length == 3) break;
                if ((!vowels.Contains(c) && char.IsLetter(c)) || allowedSpecial.Contains(c))
                {
                    result += c;
                }
            }

            if (result.Length < 3)
            {
                foreach (var c in clearName.Skip(1))
                {
                    if (result.Length == 3) break;
                    if (!result.Contains(c) && char.IsLetter(c)) result += c;
                }
            }

            return result.PadRight(3, '_')[..3];
        }

        private static string BuildValue(string part, CourseType type, int suffix)
            => $"{part}{type}-{suffix:D3}";

        public override string ToString() => Value;
    }
}
