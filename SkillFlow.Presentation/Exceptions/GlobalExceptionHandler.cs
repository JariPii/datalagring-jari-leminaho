using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SkillFlow.Domain.Exceptions;

namespace SkillFlow.Presentation.Exceptions
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            //logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            var (statusCode, title, logAsError) = exception switch
            {
                // Custom 404
                AttendeeNotFoundException or
                EmailNotFoundException or
                CompetenceNotFoundException or
                CourseNotFoundException or
                LocationNotFoundException or
                CourseSessionNotFoundException
                    => (StatusCodes.Status404NotFound, "Resource not found", false),

                // Custom 409
                EmailAlreadyExistsException or
                CompetenceAllreadyAssignedException or
                CompetenceNameAllreadyExistsException or
                CourseCodeAllreadyExistsException or
                CourseNameAllreadyExistsException or
                LocationNameAllreadyExistsException or
                InstructorAlreadyExistsException or
                ConcurrencyException or 
                Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
                    => (StatusCodes.Status409Conflict, "Resource conflict", false),

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
                InstructorIsMissingCompetenceException or
                MissingRowVersionException
                    => (StatusCodes.Status400BadRequest, "Business Rule Violation", false),

                // Custom 422
                CourseInUseException or
                LocationInUseException or
                LocationHasCourseSessionException or
                InstructorHasActiveSessionsException or
                StudentHasActiveEnrollmentsException
                    => (StatusCodes.Status422UnprocessableEntity, "Dependency Conflict", false),

                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", true)
            };

            if (logAsError)
                logger.LogError(exception, "Exception occorred: {Message}", exception.Message);
            else
                logger.LogWarning(exception, "Request failed: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Type = $"https://httpstatuses.com/{statusCode}",
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["errorCode"] = exception.GetType().Name;
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;


            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
