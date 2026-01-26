using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Courses
{
    public record CreateCourseDTO
    {
        public string CourseName { get; init; } = string.Empty;

        public string CourseDescription { get; init; } = string.Empty;

        public string CityPart { get; init; } = string.Empty;

        public int Year { get; init; }
    }
}
