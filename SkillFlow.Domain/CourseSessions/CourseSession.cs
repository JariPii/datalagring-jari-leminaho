using SkillFlow.Domain.Attendees;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Locations;
using SkillFlow.Domain.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Domain.CourseSessions
{
    public class CourseSession : BaseEntity
    {
        private readonly List<Enrollment> _enrollments = new();
        private readonly List<Instructor> _instructors = new();

        public CourseSessionId Id { get; private set; }
        public CourseCode CourseCode { get; private set; }
        public Course Course { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Capacity { get; private set; }

        public const int MaxCapacity = 40;
        public LocationId LocationId { get; private set; }
        public virtual Location Location { get; private set; } = null!;

        public CourseSession(CourseSessionId id, CourseCode courseCode, DateTime startDate, DateTime endDate, int capacity, LocationId locationId)
        {
            if (endDate <= startDate) throw new ArgumentException("End date cannot be before the start date");

            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "You need atleast 1 attendee");

            if (capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity), $"Max attendees is {MaxCapacity}");

            if (locationId.Value == Guid.Empty)
                throw new ArgumentException("Loctation is needed", nameof(locationId));

            Id = id;
            CourseCode = courseCode;
            StartDate = startDate;
            EndDate = endDate;
            Capacity = capacity;
            LocationId = locationId;
        }

        private CourseSession () { }


        public virtual IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
        public virtual IReadOnlyCollection<Instructor> Instructors => _instructors.AsReadOnly();

        public void UpdateCapacity(int newCapacity)
        {
            if (newCapacity > MaxCapacity)
                throw new ArgumentException(nameof(newCapacity), $"Max capacity for this course session is {MaxCapacity}");

            if (newCapacity < 1)
                throw new ArgumentOutOfRangeException(nameof(newCapacity), "Atleast 1 is needed");

            int approvedStudents = _enrollments.Count(e => e.Status == EnrollmentStatus.Approved);
            if (newCapacity < approvedStudents)
                throw new ArgumentException($"Can not go below {newCapacity} because {approvedStudents} are allready approved");

            if (Capacity == newCapacity) return;

            Capacity = newCapacity;
            UpdateTimeStamp();
        }

        public void AddInstructor(Instructor instructor)
        {
            ArgumentNullException.ThrowIfNull(instructor);

            if (!_instructors.Any(i => i.Id == instructor.Id))
            {
                _instructors.Add(instructor);
                UpdateTimeStamp();
            }
        }

        public void AddStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);

            if (_instructors.Count == 0)
                throw new InvalidOperationException("Atleast one instructor is needed");

            if (_enrollments.Any(e => e.StudentId == student.Id))
                throw new InvalidOperationException("Student is allready enrolled");

            var enrollment = new Enrollment(EnrollmentId.New(), student.Id, this.Id);
            _enrollments.Add(enrollment);

            UpdateTimeStamp();
        }

        public void SetEnrollmentStatus(AttendeeId studentId, EnrollmentStatus newStatus)
        {
            var enrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId) ?? throw new InvalidOperationException("Enrollment not found");

            if (newStatus == EnrollmentStatus.Approved)
            {
                int approvedStudents = _enrollments.Count(e => e.Status == EnrollmentStatus.Approved);

                if (approvedStudents >= Capacity)
                    throw new InvalidOperationException("Student can not be approved, max capacity reached");

                enrollment.Approve();
            }
            else if (newStatus == EnrollmentStatus.Denied)
            {
                enrollment.Deny();
            }

            UpdateTimeStamp();
        }
    }
}
