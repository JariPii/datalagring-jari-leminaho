using SkillFlow.Domain.Enums;
using System.Text.Json.Serialization;

namespace SkillFlow.Application.DTOs.Courses
{
    public record CourseDTO
    {
        public Guid Id { get; init; }

        public string CourseCode { get; init; } = string.Empty;

        public CourseType CourseType { get; init;  }

        public string CourseTypeName { get; init; } = string.Empty;

        public string CourseName { get; init; } = string.Empty;

        public string CourseDescription { get; init; } = string.Empty;

        public byte[] RowVersion { get; init; } = [];
    }
}
