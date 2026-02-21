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
    public class CourseSessionTests
    {
        [Fact]
        public void Create_ShouldCreateSession_WhenValid()
        {
            var session = CreateSession();

            session.Capacity.Should().Be(10);
            session.Enrollments.Should().BeEmpty();
            session.Instructors.Should().BeEmpty();
            session.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WhenEndBeforeStart_ShouldThrow()
        {
            var act = () =>
                CourseSession.Create(
                    CourseSessionId.New(),
                    CourseId.New(),
                    CourseCode.Create("Math", CourseType.BAS),
                    DateTime.Today,
                    DateTime.Today,
                    10,
                    LocationId.New());

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateCapacity_ShouldUpdate_AndSetTimestamp()
        {
            var session = CreateSession();

            session.UpdateCapacity(20);

            session.Capacity.Should().Be(20);
            session.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateCapacity_WhenSame_ShouldNotChangeTimestamp()
        {
            var session = CreateSession();

            session.UpdateCapacity(20);
            var t1 = session.UpdatedAt;

            session.UpdateCapacity(20);

            session.UpdatedAt.Should().Be(t1);
        }

        [Fact]
        public void UpdateDates_ShouldUpdate()
        {
            var session = CreateSession();

            var newStart = DateTime.Today.AddDays(10);
            var newEnd = DateTime.Today.AddDays(20);

            session.UpdateDates(newStart, newEnd);

            session.StartDate.Should().Be(newStart);
            session.EndDate.Should().Be(newEnd);
            session.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void AddInstructor_ShouldAdd()
        {
            var session = CreateSession();
            var instructor = CreateInstructor();

            session.AddInstructor(instructor);

            session.Instructors.Should().ContainSingle();
        }

        [Fact]
        public void AddInstructor_WhenDuplicate_ShouldThrow()
        {
            var session = CreateSession();
            var instructor = CreateInstructor();

            session.AddInstructor(instructor);

            var act = () => session.AddInstructor(instructor);

            act.Should().Throw<InstructorAlreadyExistsException>();
        }

        [Fact]
        public void AddStudent_ShouldAdd_WhenInstructorExists()
        {
            var session = CreateSession();
            var instructor = CreateInstructor();
            var student = CreateStudent();

            session.AddInstructor(instructor);

            session.AddStudent(student);

            session.Enrollments.Should().ContainSingle();
        }

        [Fact]
        public void AddStudent_WhenNoInstructor_ShouldThrow()
        {
            var session = CreateSession();
            var student = CreateStudent();

            var act = () => session.AddStudent(student);

            act.Should().Throw<InstructorMissingInSessionException>();
        }

        [Fact]
        public void SetEnrollmentStatus_ShouldApprove()
        {
            var session = CreateSession();
            var instructor = CreateInstructor();
            var student = CreateStudent();

            session.AddInstructor(instructor);
            session.AddStudent(student);

            session.SetEnrollmentStatus(student.Id, EnrollmentStatus.Approved);

            session.ApprovedEnrollmentsCount.Should().Be(1);
        }

        [Fact]
        public void SetEnrollmentStatus_WhenFull_ShouldThrow()
        {
            var session = CreateSession(capacity: 1);
            var instructor = CreateInstructor();

            session.AddInstructor(instructor);

            var s1 = CreateStudent();
            var s2 = CreateStudent();

            session.AddStudent(s1);
            session.SetEnrollmentStatus(s1.Id, EnrollmentStatus.Approved);

            session.AddStudent(s2);

            var act = () =>
                session.SetEnrollmentStatus(s2.Id, EnrollmentStatus.Approved);

            act.Should().Throw<CourseSessionFullException>();
        }

        // helpers

        private static CourseSession CreateSession(int capacity = 10)
            => CourseSession.Create(
                CourseSessionId.New(),
                CourseId.New(),
                CourseCode.Create("Math", CourseType.BAS),
                DateTime.Today,
                DateTime.Today.AddDays(5),
                capacity,
                LocationId.New());

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
