using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.CourseSessions
{
    public class Enrollment : BaseEntity
    {
        public Enrollment(EnrollmentId id, AttendeeId studentId, CourseSessionId courseSessionId)
        {
            Id = id;
            StudentId = studentId;
            CourseSessionId = courseSessionId;
            Status = EnrollmentStatus.Pending;
        }

        private Enrollment () { }

        public EnrollmentId Id { get; private set; } = null!;
        public EnrollmentStatus Status { get; private set; }
        public AttendeeId StudentId { get; private set; } = null!;
        public Student Student { get; private set; } = null!;
        public CourseSessionId CourseSessionId { get; private set; } = null!;
        public CourseSession CourseSession { get; private set; } = null!;

        public void Approve()
        {
            Status = EnrollmentStatus.Approved;
            UpdateTimeStamp();
        }

        public void Deny()
        {
            Status = EnrollmentStatus.Denied;
            UpdateTimeStamp();
        }
    }
}
