using SkillFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public EnrollmentStatus Status { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int CourseSessionId { get; set; }
        public CourseSession CourseSession { get; set; } = null!;
    }
}
