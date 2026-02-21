using FluentAssertions;
using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using Xunit;

namespace SkillFlow.Tests.Domain.CourseSessions
{
    public class EnrollmentStatusTests
    {
        [Fact]
        public void SetEnrollmentStatus_ToPending_ShouldThrow()
        {
            var session = CreateSessionWithInstructor();
            var student = CreateStudent();

            session.AddStudent(student);

            var act = () => session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Pending);

            act.Should().Throw<InvalidEnrollmentStatusException>()
               .WithMessage("*pending*");
        }

        [Fact]
        public void SetEnrollmentStatus_WhenStudentNotEnrolled_ShouldThrow()
        {
            var session = CreateSessionWithInstructor();
            var student = CreateStudent();

            var act = () => session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Approved);

            act.Should().Throw<StudentNotEnrolledException>();
        }

        [Fact]
        public void SetEnrollmentStatus_ToApproved_ShouldIncreaseApprovedCount()
        {
            var session = CreateSessionWithInstructor();
            var student = CreateStudent();

            session.AddStudent(student);

            session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Approved);

            session.ApprovedEnrollmentsCount.Should().Be(1);
        }

        [Fact]
        public void SetEnrollmentStatus_ToDenied_ShouldNotIncreaseApprovedCount()
        {
            var session = CreateSessionWithInstructor();
            var student = CreateStudent();

            session.AddStudent(student);

            session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Denied);

            session.ApprovedEnrollmentsCount.Should().Be(0);
        }

        [Fact]
        public void SetEnrollmentStatus_WhenAlreadySameStatus_ShouldBeNoOp()
        {
            var session = CreateSessionWithInstructor();
            var student = CreateStudent();

            session.AddStudent(student);

            session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Approved);
            var t1 = session.UpdatedAt;

            session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Approved);

            session.UpdatedAt.Should().Be(t1);
            session.ApprovedEnrollmentsCount.Should().Be(1);
        }

        [Fact]
        public void SetEnrollmentStatus_ToApproved_WhenFull_ShouldThrow()
        {
            var session = CreateSessionWithInstructor(capacity: 1);

            var s1 = CreateStudent();
            var s2 = CreateStudent();

            session.AddStudent(s1);
            session.SetEnrollmentStatus(s1.Id, EnrollmentStatus.Approved);

            session.AddStudent(s2);

            var act = () => session.SetEnrollmentStatus(s2.Id, EnrollmentStatus.Approved);

            act.Should().Throw<CourseSessionFullException>();
        }

        private static CourseSession CreateSessionWithInstructor(int capacity = 10)
        {
            var session = CourseSession.Create(
                CourseSessionId.New(),
                CourseId.New(),
                CourseCode.Create("Math", CourseType.BAS),
                DateTime.Today,
                DateTime.Today.AddDays(5),
                capacity,
                LocationId.New());

            session.AddInstructor(CreateInstructor());
            return session;
        }

        private static Instructor CreateInstructor()
            => (Instructor)Attendee.CreateInstructor(
                Email.Create("i@test.com"),
                AttendeeName.Create("Ada", "Lovelace"),
                null);

        private static Student CreateStudent()
            => (Student)Attendee.CreateStudent(
                Email.Create(Guid.NewGuid() + "@test.com"),
                AttendeeName.Create("Test", "Student"),
                null);
    }
}
