using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.Interfaces;
using SkillFlow.Presentation.Filters;

namespace SkillFlow.Presentation.Endpoints
{
    public static class AttendeeEndpoints
    {
        public static RouteGroupBuilder MapAttenteesEnpoints(this IEndpointRouteBuilder app)
        {
            var attendees = app.MapGroup("/api/attendees");

            attendees.MapGet("/", async (IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetAllAttendeesAsync(ct)));

            attendees.MapGet("/instructors", async (IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetAllInstructorsAsync(ct)));

            attendees.MapGet("/instructors/paged", async (int page, int pageSize, string? q, IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetInstructorsPagedAsync(page, pageSize, q, ct)));

            attendees.MapGet("/students/paged", async (int page, int pageSize, string? q, IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetStudentsPagedAsync(page, pageSize, q, ct)));

            attendees.MapGet("/search", async (string q, IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.SearchAttendeesByNameAsync(q, ct)));

            attendees.MapGet("/instructors/competence/{name}", async (string name, IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetInstructorsByCompetenceAsync(name, ct)));

            attendees.MapGet("/by-email", async (string email, IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetAttendeeByEmailAsync(email, ct)));

            attendees.MapGet("/{id:guid}", async (Guid id, IAttendeeService service, CancellationToken ct)
                => Results.Ok(await service.GetAttendeeByIdAsync(id, ct)));

            attendees.MapPost("/", async (CreateAttendeeDTO dto, IAttendeeService service, CancellationToken ct) =>
            {
                var result = await service.CreateAttendeeAsync(dto, ct);
                return Results.Created($"/api/attendees/{result.Id}", result);
            }).ValidateBody<CreateAttendeeDTO>();

            attendees.MapPatch("/{id:guid}", async (Guid id, UpdateAttendeeDTO dto, IAttendeeService service, CancellationToken ct)
                =>
            {
                var updatedAttendee = await service.UpdateAttendeeAsync(id, dto, ct);
                return Results.Ok(updatedAttendee);
            }).ValidateBody<UpdateAttendeeDTO>();

            attendees.MapDelete("/{id:guid}", async (Guid id, IAttendeeService service, CancellationToken ct) =>
            {
                await service.DeleteAttendeeAsync(id, ct);
                return Results.NoContent();
            });

            attendees.MapPost("/{id:guid}/competences", async (Guid id, AddCompetenceDTO dto, IAttendeeService service, CancellationToken ct) =>
            {
                await service.AddCompetenceToInstructorAsync(id, dto.CompetenceName, dto.RowVersion, ct);
                return Results.Ok(new { message = $"Competence '{dto.CompetenceName}' added successfully." });
            }).ValidateBody<AddCompetenceDTO>();

            return attendees;
        }
    }
}
