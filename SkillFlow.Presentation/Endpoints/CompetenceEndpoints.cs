using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.Interfaces;
using SkillFlow.Presentation.Filters;

namespace SkillFlow.Presentation.Endpoints
{
    public static class CompetenceEndpoints
    {
        public static RouteGroupBuilder MapCompetencesEndpoints(this IEndpointRouteBuilder app)
        {
            var competences = app.MapGroup("/api/competences");

            competences.MapGet("/", async (ICompetenceService service, CancellationToken ct) =>
            Results.Ok(await service.GetAllCompetencesAsync(ct)));

            competences.MapGet("/paged", async (int page, int pageSize, string? q, ICompetenceService service, CancellationToken ct) =>
            Results.Ok(await service.GetCompetencesPagedAsync(page, pageSize, q, ct)));
            

            competences.MapGet("/{id:guid}", async (Guid id, ICompetenceService service, CancellationToken ct) =>
            Results.Ok(await service.GetCompetenceDetailsAsync(id, ct)));

            competences.MapPost("/", async (CreateCompetenceDTO dto, ICompetenceService service, CancellationToken ct) =>
            {
                var result = await service.CreateCompetenceAsync(dto, ct);
                return Results.Created($"/api/competences/{result.Id}", result);
            }).ValidateBody<CreateCompetenceDTO>();

            competences.MapPatch("/{id:guid}", async (Guid id, UpdateCompetenceDTO dto, ICompetenceService service, CancellationToken ct) =>
            {
                var updateCompetence = await service.UpdateCompetenceAsync(id, dto, ct);
                return Results.Ok(updateCompetence);
            }).ValidateBody<UpdateCompetenceDTO>();

            competences.MapDelete("/{id:guid}", async (Guid id, ICompetenceService service, CancellationToken ct) =>
            {
                await service.DeleteCompetenceAsync(id, ct);
                return Results.NoContent();
            });

            return competences;
        }
    }
}
