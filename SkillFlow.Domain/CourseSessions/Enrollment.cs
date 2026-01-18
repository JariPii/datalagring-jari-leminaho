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
            if (id.Value == Guid.Empty)
                throw new ArgumentException("Enrollment Id can not be empty", nameof(id));

            if (studentId.Value == Guid.Empty)
                throw new ArgumentException("Student Id can not be empty", nameof(studentId));

            if (courseSessionId.Value == Guid.Empty)
                throw new ArgumentException("Course session Id can not be empty", nameof(courseSessionId));

            Id = id;
            StudentId = studentId;
            CourseSessionId = courseSessionId;
            Status = EnrollmentStatus.Pending;
        }

        private Enrollment () { }

        public EnrollmentId Id { get; private set; }
        public EnrollmentStatus Status { get; private set; }
        public AttendeeId StudentId { get; private set; }
        public Student Student { get; private set; } = null!;
        public CourseSessionId CourseSessionId { get; private set; }
        public CourseSession CourseSession { get; private set; } = null!;

        public void Approve()
        {
            if (Status == EnrollmentStatus.Approved) return;

            Status = EnrollmentStatus.Approved;
            UpdateTimeStamp();
        }

        public void Deny()
        {
            if (Status == EnrollmentStatus.Denied) return;

            Status = EnrollmentStatus.Denied;
            UpdateTimeStamp();
        }
    }
}
