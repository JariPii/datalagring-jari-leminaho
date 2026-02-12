using Microsoft.EntityFrameworkCore;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Interfaces;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Application.Services.Courses;
using SkillFlow.Application.Services.Locations;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure;
using System.Data;

namespace SkillFlow.Application.Services.CourseSessions
{
    public class CourseSessionService(
        ICourseSessionRepository sessionRepository,
        IAttendeeRepository attendeeRepository,
        ICourseRepository courseRepository,
        ILocationRepository locationRepository,
        SkillFlowDbContext context
        ) : ICourseSessionService
    {
        public async Task AddInstructorToCourseSessionAsync(Guid sessionId, Guid instructorId, byte[] rowVersion, CancellationToken ct)
        {
            var courseSessionId = new CourseSessionId(sessionId);
            var attendeeId = new AttendeeId(instructorId);

            var courseSession = await sessionRepository.GetByIdAsync(courseSessionId, ct) ??
                throw new CourseSessionNotFoundException(courseSessionId);

            var attendee = await attendeeRepository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            if(attendee is not Instructor instructor)
            {
                throw new InvalidRoleException(attendeeId, Role.Instructor);
            }

            courseSession.AddInstructor(instructor);

            await sessionRepository.UpdateAsync(courseSession, rowVersion, ct);

        }

        public async Task<CourseSessionDTO> CreateCourseSessionAsync(CreateCourseSessionDTO dto, CancellationToken ct)
        {
            if (dto.InstructorIds is null || dto.InstructorIds.Count == 0)
                throw new InstructorIsRequiredException("Instructor is required");

            var courseCode = CourseCode.FromValue(dto.CourseCode);
            var locationName = LocationName.Create(dto.LocationName);

            var course = await courseRepository.GetByCourseCodeAsync(courseCode, ct) ??
                throw new CourseNotFoundException(courseCode);

            var location = await locationRepository.GetByLocationNameAsync(locationName, ct) ??
                throw new LocationNotFoundException(locationName);

            var session = CourseSession.Create(
                CourseSessionId.New(),
                course.Id,
                courseCode,
                dto.StartDate,
                dto.EndDate,
                dto.Capacity,
                location.Id
                );

            foreach(var instructorId in dto.InstructorIds.Distinct())
            {
                var attendeeId = new AttendeeId(instructorId);

                var attendee = await attendeeRepository.GetByIdAsync(attendeeId, ct) ??
                    throw new AttendeeNotFoundException(attendeeId);

                if (attendee is not Instructor instructor)
                    throw new InvalidRoleException(attendeeId, Role.Instructor);

                session.AddInstructor(instructor);
            }

            await sessionRepository.AddAsync(session, ct);

            return await GetCourseSessionByIdAsync(session.Id.Value, ct);
        }

        public async Task DeleteCourseSessionAsync(Guid id, CancellationToken ct)
        {
            var courseSessionId = new CourseSessionId(id);

            var success = await sessionRepository.DeleteAsync(courseSessionId, ct);

            if (!success)
                throw new CourseSessionNotFoundException(courseSessionId);
        }

        public async Task EnrollStudentAsync(Guid sessionId, Guid studentId, CancellationToken ct)
        {
            var courseSessionId = new CourseSessionId(sessionId);
            var attendeeId = new AttendeeId(studentId);

            var session = await sessionRepository.GetByIdWithInstructorsAndEnrollmentsAsync(courseSessionId, ct) ??
                throw new CourseSessionNotFoundException(courseSessionId);

            var attendee = await attendeeRepository.GetByIdAsync(attendeeId, ct) ??
                throw new AttendeeNotFoundException(attendeeId);

            if (attendee is not Student student)
                throw new InvalidRoleException(attendeeId, Role.Student);

            session.AddStudent(student);

            await context.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<CourseSessionDTO>> GetAllCourseSessionsAsync(CancellationToken ct)
        {
            var courseSessions = await sessionRepository.GetAllAsync(ct);
            return [.. courseSessions.Select(MapToDTO)];
        }

        public async Task<IEnumerable<CourseSessionDTO>> GetAvailableCourseSessionsAsync(CancellationToken ct)
        {
            var sessions = await sessionRepository.GetSessionsWithAvailableCapacityAsync(ct);

            return [.. sessions.Select(MapToDTO)];
        }

        public async Task<CourseSessionDTO> GetCourseSessionByIdAsync(Guid id, CancellationToken ct)
        {
            var sessionId = new CourseSessionId(id);

            var session = await sessionRepository.GetByIdWithInstructorsAndEnrollmentsAsync(sessionId, ct) ??
                throw new CourseSessionNotFoundException(sessionId);

            return MapToDTO(session);
        }

        public async Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByDateAsync(DateTime date, CancellationToken ct)
        {
            var sessions = await sessionRepository.SearchByStartDateAsync(date, ct);

            return [.. sessions.Select(MapToDTO)];
        }

        public async Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByLocationAsync(Guid locationId, CancellationToken ct)
        {
            var id = new LocationId(locationId);
            var sessions = await sessionRepository.GetByLocationAsync(id, ct);

            return [.. sessions.Select(MapToDTO)];
        }

        public async Task<IReadOnlyList<EnrollmentDTO>> GetEnrollmentsBySessionIdAsync(Guid sessionId, CancellationToken ct = default)
        {
            var id = new CourseSessionId(sessionId);
            var session = await sessionRepository.GetByIdWithInstructorsAndEnrollmentsAsync(id, ct) ??
                throw new CourseSessionNotFoundException(id);

            return [.. session.Enrollments.Select(e => new EnrollmentDTO {
                StudentId = e.StudentId.Value,
                StudentName = e.Student.Name.Fullname,
                Status = e.Status,
                EnrolledAt = e.CreatedAt
            })];
        }

        public async Task<IEnumerable<CourseSessionDTO>> SearchCourseSessionsAsync(string searchTerm, CancellationToken ct)
        {
            var sessions = await sessionRepository.SearchAsync(searchTerm, ct);
            return [.. sessions.Select(MapToDTO)];
        }

        public async Task SetEnrollmentStatusAsync(Guid sessionId, Guid studentId, EnrollmentStatus status, byte[] rowVersion, CancellationToken ct)
        {
            using var transaction = await context.Database.BeginTransactionAsync(ct);

            try
            {
                var session = await sessionRepository.GetByIdWithInstructorsAndEnrollmentsAsync(new CourseSessionId(sessionId), ct) ??
                    throw new CourseSessionNotFoundException(new CourseSessionId(sessionId));

                session.SetEnrollmentStatus(new AttendeeId(studentId), status);

                await sessionRepository.UpdateAsync(session, rowVersion, ct);

                await transaction.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(ct);
                throw new ConcurrencyException();
            }
        }

        public async Task<CourseSessionDTO> UpdateCourseSessionAsync(UpdateCourseSessionDTO dto, CancellationToken ct)
        {
            var id = new CourseSessionId(dto.Id);

            var session = await sessionRepository.GetByIdWithInstructorsAndEnrollmentsAsync(id, ct) ??
                throw new CourseSessionNotFoundException(id);

            if (dto.Capacity.HasValue)
                session.UpdateCapacity(dto.Capacity.Value);

            if (dto.StartDate.HasValue || dto.EndDate.HasValue)
                session.UpdateDates(dto.StartDate ?? session.StartDate, dto.EndDate ?? session.EndDate);

            await sessionRepository.UpdateAsync(session, dto.RowVersion, ct);

            return MapToDTO(session);
        }

        private static CourseSessionDTO MapToDTO(CourseSession courseSession)
        {
            return new CourseSessionDTO
            {
                Id = courseSession.Id.Value,
                CourseCode = courseSession.CourseCode.Value,
                Course = CourseService.MapToDTO(courseSession.Course),
                Location = LocationService.MapToDTO(courseSession.Location),
                Capacity = courseSession.Capacity,
                StartDate = courseSession.StartDate.ToLocalTime(),
                EndDate = courseSession.EndDate.ToLocalTime(),
                ApprovedEnrollmentsCount = courseSession.ApprovedEnrollmentsCount,
                Instructors = [.. courseSession.Instructors.Select(AttendeeService.MapToDTO)],
                RowVersion = courseSession.RowVersion
            };
        }
    }
}
