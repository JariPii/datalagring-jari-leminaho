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
            if (id.Value == Guid.Empty)
                throw new ArgumentException("Course id can not be empty", nameof(id));

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

        public void AddInstructor(Instructor instructor)
        {
            if (instructor == null) throw new ArgumentNullException(nameof(instructor));
            if (!_instructors.Contains(instructor))
            {
                _instructors.Add(instructor);
                UpdateTimeStamp();
            }
        }

        public void AddStudent(Student student)
        {
            if (!_instructors.Any())
                throw new InvalidOperationException("Atleast one instructor is needed");

            if (_enrollments.Count >= Capacity)
                throw new InvalidOperationException("Course is fully booked");

            if (_enrollments.Any(e => e.StudentId == student.Id))
                throw new InvalidOperationException("Student is allready enrolled");

            var enrollment = new Enrollment(EnrollmentId.New(), student.Id, this.Id);
            _enrollments.Add(enrollment);

            UpdateTimeStamp();
        }
    }
}
