using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.CourseSessions
{
    public class CourseSession : BaseEntity<CourseSessionId>, IAggregateRoot
    {
        private readonly List<Enrollment> _enrollments = new();
        private readonly List<Instructor> _instructors = new();

        public CourseId CourseId { get; private set; }
        public CourseCode CourseCode { get; private set; }
        public Course Course { get; private set; } = null!;
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int Capacity { get; private set; }

        public const int MaxCapacity = 40;
        public LocationId LocationId { get; private set; }
        public virtual Location Location { get; private set; } = null!;

        private CourseSession(CourseSessionId id, CourseId courseId, CourseCode courseCode, DateTime startDate, DateTime endDate, int capacity, LocationId locationId)
        {          

            Id = id;
            CourseId = courseId;
            CourseCode = courseCode;
            StartDate = startDate;
            EndDate = endDate;
            Capacity = capacity;
            LocationId = locationId;
        }

        public static CourseSession Create(CourseSessionId id, CourseId courseId, CourseCode code, DateTime startDate, DateTime endDate, int capacity, LocationId locationId)
        {
            if (endDate <= startDate) throw new ArgumentException("End date cannot be before the start date");

            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity), "You need atleast 1 attendee");

            if (capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity), $"Max attendees is {MaxCapacity}");

            if (locationId.Value == Guid.Empty)
                throw new ArgumentException("Loctation is needed", nameof(locationId));

            return new CourseSession(
                id,
                courseId,
                code,
                startDate,
                endDate,
                capacity,
                locationId
                );
        }

        private CourseSession () { }


        public virtual IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
        public virtual IReadOnlyCollection<Instructor> Instructors => _instructors.AsReadOnly();

        public int ApprovedEnrollmentsCount => _enrollments.Count(e => e.Status == EnrollmentStatus.Approved);

        public void UpdateCapacity(int newCapacity)
        {
            if (newCapacity < 1)
                throw new InvalidCapacityException("Capacity must be atleast 1");

            CheckCapacity(newCapacity);

            if (Capacity == newCapacity) return;

            Capacity = newCapacity;
            UpdateTimeStamp();
        }

        public void UpdateDates(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new InvalidSessionDatesException(start, end);

            if (StartDate == start && EndDate == end) return;

            StartDate = start;
            EndDate = end;
            UpdateTimeStamp();
        }

        public void UpdateLocation(LocationId newLocationID)
        {
            if (newLocationID.Value == Guid.Empty)
                throw new LocationNotFoundException(newLocationID);

            if (LocationId == newLocationID) return;

            LocationId = newLocationID;
            UpdateTimeStamp();
        }

        public void UpdateCourse(CourseId newCourseId, CourseCode newCourseCode)
        {
            if (newCourseId.Value == Guid.Empty)
                throw new CourseNotFoundException(newCourseId);

            if (CourseId == newCourseId && CourseCode.Equals(newCourseCode)) return;

            CourseId = newCourseId;
            CourseCode = newCourseCode;
            UpdateTimeStamp();
        }

        public void SetInstructors(IEnumerable<Instructor> instructors)
        {
            if (instructors is null)
                throw new InstructorIsRequiredException("Instructor is required");

            var list = instructors.ToList();

            if (list.Count == 0)
                throw new InstructorIsRequiredException("At least one instructor is required");

            if (list.GroupBy(i => i.Id).Any(g => g.Count() > 1))
                throw new InstructorAlreadyExistsException(list.First().Id, this.Id);

            var currentIds = _instructors.Select(x => x.Id.Value).OrderBy(x => x).ToArray();
            var newIds = list.Select(x => x.Id.Value).OrderBy(x => x).ToArray();

            if (currentIds.SequenceEqual(newIds)) return;

            _instructors.Clear();
            _instructors.AddRange(list);
            UpdateTimeStamp();
        }

        public void AddInstructor(Instructor instructor)
        {
            if (instructor is null)
                throw new InstructorIsRequiredException("Instructor is required");

            if (_instructors.Any(i => i.Id == instructor.Id))
                throw new InstructorAlreadyExistsException(instructor.Id, this.Id);

            _instructors.Add(instructor);
            UpdateTimeStamp();
        }

        public void AddStudent(Student student)
        {
            if (student is null)
                throw new StudentIsRequiredException();

            if (_instructors.Count == 0)
                throw new InstructorMissingInSessionException(this.Id);

            if (_enrollments.Any(e => e.StudentId == student.Id))
                throw new StudentAllreadyEnrolledException(student.Id, this.Id);

            var enrollment = new Enrollment(EnrollmentId.New(), student.Id, this.Id);
            _enrollments.Add(enrollment);

            UpdateTimeStamp();
        }

        public void CheckCapacity(int requestedCapacity)
        {
            if (requestedCapacity < ApprovedEnrollmentsCount)
                throw new InvalidCapacityException($"Can not lower capacity to {requestedCapacity} because there is {ApprovedEnrollmentsCount}");

            if (requestedCapacity > MaxCapacity)
                throw new InvalidCapacityException($"Max capacity is {MaxCapacity}");
        }

        public void SetEnrollmentStatus(AttendeeId studentId, EnrollmentStatus newStatus)
        {
            if (newStatus == EnrollmentStatus.Pending)
                throw new InvalidEnrollmentStatusException("Can not set status to pending");

            var enrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId) ?? throw new StudentNotEnrolledException(studentId, this.Id);

            if (enrollment.Status == newStatus) return;

            if (newStatus == EnrollmentStatus.Approved)
            {
                if (ApprovedEnrollmentsCount >= Capacity)
                    throw new CourseSessionFullException(Capacity);

                enrollment.Approve();
                UpdateTimeStamp();
                return;
            }
            else if (newStatus == EnrollmentStatus.Denied)
            {
                enrollment.Deny();
                UpdateTimeStamp();
                return;
            }

            throw new InvalidEnrollmentStatusException($"Invalid status: {newStatus}");
        }
    }
}
