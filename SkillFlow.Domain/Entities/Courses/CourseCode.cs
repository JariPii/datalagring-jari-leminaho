using SkillFlow.Domain.Enums;

namespace SkillFlow.Domain.Entities.Courses
{

    public readonly record struct CourseCode
    {
        public string Value => $"{CoursePart}{CourseType}-{CourseSuffix:D3}";
        public string CoursePart { get; init; }
        public CourseType CourseType { get; init; }
        public int CourseSuffix { get; init; }

        private CourseCode ( string course, CourseType type, int suffix)
        {
            CoursePart = course;
            CourseType = type;
            CourseSuffix = suffix;
        }


        public static CourseCode Create(string coursePartialName, CourseType type, int suffix = 10)
        {

            if (string.IsNullOrWhiteSpace(coursePartialName))
                throw new ArgumentException("Course value can not be empty", nameof(coursePartialName));

            if (!Enum.IsDefined(type))
                throw new ArgumentException("Invalid course type", nameof(type));

            if (suffix <= 0)
                throw new ArgumentException("Suffix is needed, greater than 0", nameof(suffix));

            if (suffix % 10 != 0)
                throw new ArgumentException("Suffix must be a multiple of 10", nameof(suffix));

            if (suffix > 990)
                throw new ArgumentException("Suffix must be between 010 and 990", nameof(suffix));


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

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value can not be empty", nameof(value));

            value = value.Trim();

            if (value.Length != 9 || value[5] != '-')
                throw new ArgumentException("Invalid course code format. Expected e.g. MAGRD-010", nameof(value));

            var part = value[..2];
            var typeStr = value.Substring(2, 3);
            var suffixStr = value.Substring(6, 3);

            if (!part.All(c => char.IsLetter(c) || c == '_'))
                throw new ArgumentException("Invalid coursepart in course code. Expected 2 letters", nameof(value));

            if (!Enum.TryParse<CourseType>(typeStr, ignoreCase: true, out var type))
                throw new ArgumentException("Invalid course type in course code", nameof(value));

            if (!suffixStr.All(char.IsDigit))
                throw new ArgumentException("Invalid suffix in course code. Expected 3 digits.", nameof(value));

            //var suffix = int.Parse(suffixStr);

            if (!int.TryParse(suffixStr, out var suffix) || suffix <= 0)
                throw new ArgumentException("Invalid suffix in course code", nameof(value));

            if (suffix <= 0 || suffix % 10 != 0 || suffix > 990)
                throw new ArgumentException("Invalid suffix in course code. Expected value is 010-990 in steps of 10", nameof(value));

            return new CourseCode(part.ToUpperInvariant(), type, suffix);
        }

        //private static string GetPart(string name)
        //    => (name.Length >= 2 ? name[..2] : name.PadRight(2, '0')).ToUpperInvariant();

        private static string GetPart(string name)
            => name.Trim()
            .PadRight(2, '_')
            .Substring(0, 2)
            .ToUpperInvariant();

        private static string BuildValue(string part, CourseType type, int suffix)
            => $"{part}{type}-{suffix:D3}";

        public override string ToString() => Value;
    }
}
