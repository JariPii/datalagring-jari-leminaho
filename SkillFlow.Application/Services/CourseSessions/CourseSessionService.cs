using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Interfaces;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Entities.CourseSessions;
using SkillFlow.Domain.Enums;
using SkillFlow.Domain.Exceptions;
using SkillFlow.Domain.Interfaces;

namespace SkillFlow.Application.Services
{
    public class CourseSessionService(
        ICourseSessionRepository sessionRepository,
        IAttendeeRepository attendeeRepository,
        ICourseRepository courseRepository,
        ILocationRepository locationRepository
        ) : ICourseSessionService
    {
        public async Task AddInstructorToCourseSessionAsync(Guid sessionId, Guid instructorId, CancellationToken ct)
        {
            var courseSessionId = new CourseSessionId(sessionId);
            var attendeeId = new AttendeeId(instructorId);

            var courseSession = await sessionRepository.GetByIdAsync(courseSessionId) ??
                throw new CourseSessionNotFoundException(courseSessionId);

            var attendee = await attendeeRepository.GetByIdAsync(attendeeId) ??
                throw new AttendeeNotFoundException(attendeeId);

            if(attendee is not Instructor instructor)
            {
                throw new InvalidRoleException(attendeeId, Role.Instructor);
            }

            courseSession.AddInstructor(instructor);

            await sessionRepository.UpdateAsync(courseSession);

        }

        public Task<CourseSessionDTO> CreateCourseSessionAsync(CreateCourseSessionDTO dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCourseSessionAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task EnrollStudentAsync(Guid sessionId, Guid studentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseSessionDTO>> GetAllCourseSessionsAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<CourseSessionDTO>> SearchCourseSessionsAsync(string searchTerm, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SetEnrollmentStatusAsync(Guid sessionId, Guid studentId, string status, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCourseSessionAsync(UpdateCourseSessionDTO dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
