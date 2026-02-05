using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.CourseSessions
{
    public class Enrollment : BaseEntity<EnrollmentId>
    {
        public Enrollment(EnrollmentId id, AttendeeId studentId, CourseSessionId courseSessionId)
        {
            Id = id;
            StudentId = studentId;
            CourseSessionId = courseSessionId;
            Status = EnrollmentStatus.Pending;
        }

        private Enrollment () { }

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
