using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.CourseSessions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class InstructorCourseSession
    {
        public Guid InstructorId { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public int CourseSessionId { get; set; }
        public CourseSession CourseSession { get; set; } = null!;
    }
}
