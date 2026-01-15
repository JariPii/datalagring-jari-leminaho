using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class CourseSession : BaseIdEntity<int>
    {
        public string CourseCode { get; set; } = null!;
        public Course Course { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Capacity { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;
        public ICollection<InstructorCourseSession> InstructorCourseSessions { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
    }
}
