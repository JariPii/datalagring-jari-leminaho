using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkillFlow.Domain.Exceptions;

namespace SkillFlow.Presentation.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var (statusCode, title) = exception switch
            {
                // Custom 404
                AttendeeNotFoundException or
                EmailNotFoundException or
                CompetenceNotFoundException or
                CourseNotFoundException or
                LocationNotFoundException or
                CourseSessionNotFoundException
                    => (StatusCodes.Status404NotFound, "Resource not found"),

                // Custom 409
                EmailAlreadyExistsException or
                CompetenceAllreadyAssignedException or
                CompetenceNameAllreadyExistsException or
                CourseCodeAllreadyExistsException or
                CourseNameAllreadyExistsException or
                LocationNameAllreadyExistsException or
                InstructorAlreadyExistsException
                    => (StatusCodes.Status409Conflict, "Resource conflict"),

                // Custom 400
                InvalidRoleException or
                InvalidPhoneNumberException or
                InvalidNameException or
                InvalidEmailException or
                InvalidCourseNameException or
                InvalidLocationNameException or
                InvalidCompetenceNameException or
                InvalidCapacityException or
                InvalidSessionDatesException or
                CourseSessionFullException or
                InstructorMissingInSessionException or
                AttendeeIsRequiredException or
                StudentIsRequiredException or
                InstructorIsRequiredException or
                InstructorIsMissingCompetenceException
                    => (StatusCodes.Status400BadRequest, "Business Rule Violation"),

                // Custom 422
                CourseInUseException or
                LocationInUseException or
                LocationHasCourseSessionException or
                InstructorHasActiveSessionsException or
                StudentHasActiveEnrollmentsException
                    => (StatusCodes.Status422UnprocessableEntity, "Dependency Conflict"),

                ConcurrencyException or Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
                    => (StatusCodes.Status409Conflict, "Concurrency Conflict"),

                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            var problemDetails = new ProblemDetails()
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Type = exception.GetType().Name,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
