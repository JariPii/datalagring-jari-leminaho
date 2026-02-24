using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.Interfaces;
using SkillFlow.Presentation.Filters;

namespace SkillFlow.Presentation.Endpoints
{
    public static class CourseSessionEndpoints
    {
        public static RouteGroupBuilder MapCourseSessionsEndpoints(this IEndpointRouteBuilder app)
        {
            var courseSessions = app.MapGroup("/api/courseSessions");

            courseSessions.MapGet("/", async (ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.GetAllCourseSessionsAsync(ct)));

            courseSessions.MapGet("/paged", async (int page, int pageSize, string? q, ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.GetCourseSessionsPagedAsync(page, pageSize, q, ct)));

            courseSessions.MapGet("/{id:guid}", async (Guid id, ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.GetCourseSessionByIdAsync(id, ct)));

            courseSessions.MapGet("/available", async (ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.GetAvailableCourseSessionsAsync(ct)));

            courseSessions.MapGet("/search", async (string searchTerm, ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.SearchCourseSessionsAsync(searchTerm, ct)));

            courseSessions.MapGet("/date/{date:datetime}", async (DateTime date, ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.GetCourseSessionsByDateAsync(date, ct)));

            courseSessions.MapGet("/location/{locationId:guid}", async (Guid locationId, ICourseSessionService service, CancellationToken ct) =>
                Results.Ok(await service.GetCourseSessionsByLocationAsync(locationId, ct)));

            courseSessions.MapPost("/", async (CreateCourseSessionDTO dto, ICourseSessionService service, CancellationToken ct) =>
            {
                var result = await service.CreateCourseSessionAsync(dto, ct);
                return Results.Created($"/api/courseSessions/{result.Id}", result);
            }).ValidateBody<CreateCourseSessionDTO>();

            courseSessions.MapPatch("/{id:guid}", async (Guid id, UpdateCourseSessionDTO dto, ICourseSessionService service, CancellationToken ct) =>
            {
                var updated = await service.UpdateCourseSessionAsync(id, dto, ct);
                return Results.Ok(updated);
            }).ValidateBody<UpdateCourseSessionDTO>();

            courseSessions.MapDelete("/{id:guid}", async (Guid id, ICourseSessionService service, CancellationToken ct) =>
            {
                await service.DeleteCourseSessionAsync(id, ct);
                return Results.NoContent();
            });

            courseSessions.MapPost("/{id:guid}/instructors", async (Guid id, AddInstructorToCourseSessionDTO dto, ICourseSessionService service, CancellationToken ct) =>
            {
                await service.AddInstructorToCourseSessionAsync(id, dto.InstructorId, dto.RowVersion, ct);
                return Results.Ok();
            }).ValidateBody<AddInstructorToCourseSessionDTO>();

            courseSessions.MapPost("/{id:guid}/enrollments", async (Guid id, EnrollStudentDTO dto, ICourseSessionService service, CancellationToken ct) =>
            {
                await service.EnrollStudentAsync(id, dto.StudentId, dto.RowVersion, ct);
                return Results.Ok();
            }).ValidateBody<EnrollStudentDTO>();

            courseSessions.MapGet("/{id:guid}/enrollments", async (Guid id, ICourseSessionService service, CancellationToken ct) => Results.Ok(await service.GetEnrollmentsBySessionIdAsync(id, ct)));

            courseSessions.MapPut("/{id:guid}/enrollment/{studentId:guid}/status", async (Guid id, Guid studentId, UpdateEnrollmentStatusDTO dto, ICourseSessionService service, CancellationToken ct) =>
            {
                await service.SetEnrollmentStatusAsync(id, studentId, dto.NewStatus, dto.RowVersion, ct);
                return Results.Ok();
            }).ValidateBody<UpdateEnrollmentStatusDTO>();

            return courseSessions;
        }
    }
}
