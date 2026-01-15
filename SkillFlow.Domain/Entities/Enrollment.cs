using SkillFlow.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Enrollment : BaseIdEntity<int>
    {
        public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Pending;
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public int CourseSessionId { get; set; }
        public CourseSession CourseSession { get; set; } = null!;

        public void Approve()
        {
            Status = EnrollmentStatus.Approved;
        }
    }
}
