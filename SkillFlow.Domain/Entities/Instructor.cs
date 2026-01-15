using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class Instructor
    {
        public Guid AttendeeId { get; set; }
        public Attendee Attendee { get; set; } = null!;

        public ICollection<InstructorCompetence> InstructorCompetences { get; set; } = [];
        public ICollection<InstructorCourseSession> InstructorCourseSessions { get; set; } = [];
    }
}
