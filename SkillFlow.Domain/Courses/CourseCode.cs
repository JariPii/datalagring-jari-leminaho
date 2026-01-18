using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Courses
{

    public readonly record struct CourseCode
    {
        private const int RequiredLength = 12;
        public string Value { get; init; }

        private CourseCode(string value) => Value = value;

        public static CourseCode Create(string locationPartialName, string coursePartialName, int year, int suffix = 1)
        {

            string l = locationPartialName.Substring(0, 2).ToUpper();
            string c = coursePartialName.Substring(0, 2).ToUpper();

            string generatedCourseCode = $"{l}{c}{year}-{suffix:D3}";

            return new CourseCode(generatedCourseCode);
        }

        public static CourseCode FromValue(string value)
        {
            if(string.IsNullOrWhiteSpace(value) || value.Length != RequiredLength)
            {
                throw new ArgumentException("Invalid coursecode format");
            }

            return new CourseCode(value);
        }
    }
}
