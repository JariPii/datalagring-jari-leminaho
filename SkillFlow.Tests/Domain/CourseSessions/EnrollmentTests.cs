using FluentAssertions;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Enums;
using Xunit;

namespace SkillFlow.Tests.Domain.CourseSessions
{
    public class EnrollmentTests
    {
        [Fact]
        public void Constructor_ShouldSetPendingStatus_AndSetIds()
        {
            var id = EnrollmentId.New();
            var studentId = AttendeeId.New();
            var sessionId = CourseSessionId.New();

            var enrollment = new Enrollment(id, studentId, sessionId);

            enrollment.Id.Should().Be(id);
            enrollment.StudentId.Should().Be(studentId);
            enrollment.CourseSessionId.Should().Be(sessionId);
            enrollment.Status.Should().Be(EnrollmentStatus.Pending);
            enrollment.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Approve_WhenPending_ShouldSetApproved_AndSetUpdatedAt()
        {
            var enrollment = CreateEnrollment();

            enrollment.Approve();

            enrollment.Status.Should().Be(EnrollmentStatus.Approved);
            enrollment.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Approve_WhenAlreadyApproved_ShouldNotChangeUpdatedAt()
        {
            var enrollment = CreateEnrollment();

            enrollment.Approve();
            var t1 = enrollment.UpdatedAt;

            enrollment.Approve();

            enrollment.UpdatedAt.Should().Be(t1);
        }

        [Fact]
        public void Deny_WhenPending_ShouldSetDenied_AndSetUpdatedAt()
        {
            var enrollment = CreateEnrollment();

            enrollment.Deny();

            enrollment.Status.Should().Be(EnrollmentStatus.Denied);
            enrollment.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Deny_WhenAlreadyDenied_ShouldNotChangeUpdatedAt()
        {
            var enrollment = CreateEnrollment();

            enrollment.Deny();
            var t1 = enrollment.UpdatedAt;

            enrollment.Deny();

            enrollment.UpdatedAt.Should().Be(t1);
        }

        [Fact]
        public void Approve_AfterDeny_ShouldSwitchToApproved_AndUpdateTimestampAgain()
        {
            var enrollment = CreateEnrollment();

            enrollment.Deny();
            var t1 = enrollment.UpdatedAt;

            System.Threading.Thread.Sleep(5);

            enrollment.Approve();
            var t2 = enrollment.UpdatedAt;

            enrollment.Status.Should().Be(EnrollmentStatus.Approved);
            t2.Should().NotBeNull();
            t1.Should().NotBeNull();
            t2!.Value.Should().BeAfter(t1!.Value);
        }

        private static Enrollment CreateEnrollment()
            => new Enrollment(EnrollmentId.New(), AttendeeId.New(), CourseSessionId.New());
    }
}
