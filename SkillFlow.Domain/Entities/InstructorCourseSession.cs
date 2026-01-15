using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.Entities
{
    public class InstructorCourseSession
    {
        public int Instructor_Id { get; set; }
        public Instructor Instructor { get; set; } = null!;
        public int CourseSession_Id { get; set; }
        public CourseSession CourseSession { get; set; } = null!;
    }
}
