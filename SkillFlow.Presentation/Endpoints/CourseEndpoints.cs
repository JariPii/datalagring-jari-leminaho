using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Interfaces;
using SkillFlow.Presentation.Filters;

namespace SkillFlow.Presentation.Endpoints
{
    public static class CourseEndpoints
    {
        public static RouteGroupBuilder MapCoursesEndpoints(this IEndpointRouteBuilder app)
        {
            var courses = app.MapGroup("/api/courses");

            courses.MapGet("/", async (ICourseService service, CancellationToken ct) =>
                Results.Ok(await service.GetAllCoursesAsync(ct)));

            courses.MapGet("/search", async (string searchTerm, ICourseService service, CancellationToken ct)
                => Results.Ok(await service.SearchCoursesAsync(searchTerm, ct)));

            courses.MapGet("/{name}", async (string name, ICourseService service, CancellationToken ct)
                => Results.Ok(await service.GetCourseByNameAsync(name, ct)));

            courses.MapPost("/", async (CreateCourseDTO dto, ICourseService service, CancellationToken ct) =>
            {
                var result = await service.CreateCourseAsync(dto, ct);
                return Results.Created($"/api/courses/{result.Id}", result);
            }).ValidateBody<CreateCourseDTO>();

            courses.MapPatch("/{id:guid}", async (Guid id, UpdateCourseDTO dto, ICourseService service, CancellationToken ct) =>
            {
                var updateCourse = await service.UpdateCourseAsync(id, dto, ct);
                return Results.Ok(updateCourse);
            }).ValidateBody<UpdateCourseDTO>();

            courses.MapDelete("/{id:guid}", async (Guid id, ICourseService service, CancellationToken ct) =>
            {
                await service.DeleteCourseAsync(id, ct);
                return Results.NoContent();
            });

            return courses;
        }
    }
}
