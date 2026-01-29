using Microsoft.EntityFrameworkCore;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Entities.Locations;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure;

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
        public async Task AddInstructorToCourseSessionAsync(Guid sessionId, Guid instructorId, CancellationToken ct)
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

            await sessionRepository.UpdateAsync(courseSession, ct);

        }

        public async Task<CourseSessionDTO> CreateCourseSessionAsync(CreateCourseSessionDTO dto, CancellationToken ct)
        {
            var courseId = new CourseId(dto.CourseId);
            var locationId = new LocationId(dto.LocationId);

            var course = await courseRepository.GetByIdAsync(courseId, ct) ??
                throw new CourseNotFoundException(courseId);

            var location = await locationRepository.GetByIdAsync(locationId, ct) ??
                throw new LocationNotFoundException(locationId);

            var existingCourseCodeSuffix = await sessionRepository.CountSessionsForCourseAndYear(
                location.LocationName.Value,
                course.CourseName.Value,
                dto.StartDate.Year,
                ct);

            var sessionCode = CourseCode.Create(
                location.LocationName.Value,
                course.CourseName.Value,
                dto.StartDate.Year,
                existingCourseCodeSuffix + 1
                );

            var session = new CourseSession(
                CourseSessionId.New(),
                sessionCode,
                dto.StartDate,
                dto.EndDate,
                dto.Capacity,
                location.Id
                );

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

        public Task<IEnumerable<CourseSessionDTO>> GetAvailableCourseSessionsAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<CourseSessionDTO> GetCourseSessionByIdAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByDateAsync(DateTime date, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseSessionDTO>> GetCourseSessionsByLocationAsync(Guid locationId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<EnrollmentDTO>> GetEnrollmentsBySessionIdAsync(Guid sessionId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseSessionDTO>> SearchCourseSessionsAsync(string searchTerm, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task SetEnrollmentStatusAsync(Guid sessionId, Guid studentId, EnrollmentStatus status, CancellationToken ct)
        {
            using var transaction = await context.Database.BeginTransactionAsync(ct);

            try
            {
                var session = await sessionRepository.GetByIdWithInstructorsAndEnrollmentsAsync(new CourseSessionId(sessionId), ct) ??
                    throw new CourseSessionNotFoundException(new CourseSessionId(sessionId));

                session.SetEnrollmentStatus(new AttendeeId(studentId), status);

                await sessionRepository.UpdateAsync(session, ct);

                await transaction.CommitAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync(ct);
                throw new ConcurrencyException();
            }
        }

        public Task UpdateCourseSessionAsync(UpdateCourseSessionDTO dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private static CourseSessionDTO MapToDTO(CourseSession courseSession)
        {
            return new CourseSessionDTO
            {
                Id = courseSession.Id.Value,
                CourseCode = courseSession.CourseCode.Value,
                Course = courseSession.Course.CourseName.Value,
                Location = courseSession.Location.LocationName.Value,
                Capacity = courseSession.Capacity,
                ApprovedEnrollmentsCount = courseSession.Enrollments.Count,
                StartDate = courseSession.StartDate.ToLocalTime(),
                EndDate = courseSession.EndDate.ToLocalTime()
            };
        }
    }
}
