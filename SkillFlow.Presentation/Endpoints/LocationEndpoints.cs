using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Interfaces;
using SkillFlow.Presentation.Filters;

namespace SkillFlow.Presentation.Endpoints
{
    public static class LocationEndpoints
    {
        public static RouteGroupBuilder MapLocationsEndpoints(this IEndpointRouteBuilder app)
        {
            var locations = app.MapGroup("/api/locations");

            locations.MapGet("/", async (ILocationService service, CancellationToken ct) =>
                Results.Ok(await service.GetAllLocationsAsync(ct)));

            locations.MapGet("/{id:guid}", async (Guid id, ILocationService service, CancellationToken ct) =>
                Results.Ok(await service.GetLocationByIdAsync(id, ct)));

            locations.MapGet("/search", async (string searchTerm, ILocationService service, CancellationToken ct) =>
                Results.Ok(await service.SearchLocationsAsync(searchTerm, ct)));

            locations.MapPost("/", async (CreateLocationDTO dto, ILocationService service, CancellationToken ct) =>
            {
                var result = await service.CreateLocationAsync(dto, ct);
                return Results.Created($"/api/locations/{result.Id}", result);
            }).ValidateBody<CreateLocationDTO>();

            locations.MapPatch("/{id:guid}", async (Guid id, UpdateLocationDTO dto, ILocationService service, CancellationToken ct) =>
            {

                var updateLocation = await service.UpdateLocationAsync(id, dto, ct);
                return Results.Ok(updateLocation);
            }).ValidateBody<UpdateLocationDTO>();

            locations.MapDelete("/{id:guid}", async (Guid id, ILocationService service, CancellationToken ct) =>
            {
                await service.DeleteLocationAsync(id, ct);
                return Results.NoContent();
            });

            return locations;
        }
    }
}
