using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{

    public readonly record struct CourseCode
    {
        private const int RequiredLength = 12;
        public string Value => $"{CityPart}{CoursePart}{CourseYear}-{CourseSuffix:D3}";

        public string CityPart { get; init; }
        public string CoursePart { get; init; }
        public int CourseYear { get; init; }
        public int CourseSuffix { get; init; }

        private CourseCode (string city, string course, int year, int suffix)
        {

            CityPart = city;
            CoursePart = course;
            CourseYear = year;
            CourseSuffix = suffix;
        }


        public static CourseCode Create(string locationPartialName, string coursePartialName, int year, int suffix = 1)
        {
            if (string.IsNullOrWhiteSpace(locationPartialName))
                throw new ArgumentException("City value can not be empty", nameof(locationPartialName));

            if (string.IsNullOrWhiteSpace(coursePartialName))
                throw new ArgumentException("Course value can not be empty", nameof(coursePartialName));

            if (year < 2000 || year > 2100)
                throw new ArgumentException("Invalid course year", nameof(year));

            if (suffix <= 0)
                throw new ArgumentException("Suffix is needed, greater than 0", nameof(suffix));

                

            string l = GetPart(locationPartialName);
            string c = GetPart(coursePartialName);


            return new CourseCode(l,c,year,suffix);
        }

        private static string GetPart(string name) => (name.Length >= 2 ? name[..2] : name.PadRight(2, '0')).ToUpperInvariant();

        public static CourseCode FromValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != RequiredLength)
                throw new ArgumentException("Invalid course code length", nameof(value));

            if (value[8] != '-')
                throw new ArgumentException("Invalid course code format", nameof(value));

            var city = value[..2];
            var course = value.Substring(2, 2);
            var year = int.Parse(value.Substring(4, 4));
            var suffix = int.Parse(value.Substring(9, 3));

            return new CourseCode(city, course, year, suffix);
        }

        public override string ToString() => Value;
    }
}
