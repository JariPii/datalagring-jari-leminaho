using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Courses
{
    public record UpdateCourseDTO
    {
        public Guid Id { get; init; }

        public string CourseName { get; init; } = string.Empty;

        public string CourseDescription { get; init; } = string.Empty;
    }
}
