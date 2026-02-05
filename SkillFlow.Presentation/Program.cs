using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Interfaces;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Application.Services.Competences;
using SkillFlow.Application.Services.Courses;
using SkillFlow.Application.Services.CourseSessions;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Presentation.Exceptions;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddDbContext<SkillFlowDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

#region AddScoped

builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();
builder.Services.AddScoped<IAttendeeQueries, AttendeeRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseSessionRepository, CourseSessionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ICompetenceRepository, CompetenceRepository>();

builder.Services.AddScoped<IAttendeeService, AttendeeService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseSessionService, CourseSessionService>();
builder.Services.AddScoped<ICompetenceService, CompetenceService>();

#endregion

var app = builder.Build();

app.UseExceptionHandler();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

#region Attendees

var attendees = app.MapGroup("/api/attendees");

attendees.MapGet("/", async (IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.GetAllAttendeesAsync(ct)));

attendees.MapGet("/instructors", async (IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.GetAllInstructorsAsync(ct)));

attendees.MapGet("/students", async (IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.GetAllStudentsAsync(ct)));

attendees.MapGet("/search", async (string searchTerm, IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.SearchAttendeesByNameAsync(searchTerm, ct)));

attendees.MapGet("/instructors/competence/{name}", async (string name, IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.GetInstructorsByCompetenceAsync(name, ct)));

attendees.MapGet("/role/{role}", async (string role, IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.GetAttendeesByRoleAsync(role, ct)));

attendees.MapGet("/email/{email}", async (string email, IAttendeeService service, CancellationToken ct)
    => Results.Ok(await service.GetAttendeeByEmailAsync(email, ct)));

attendees.MapGet("/{id:guid}", async (Guid id, IAttendeeService service, CancellationToken ct) 
    => Results.Ok(await service.GetAttendeeByIdAsync(id, ct)));

attendees.MapPost("/", async (CreateAttendeeDTO dto, IAttendeeService service, CancellationToken ct) =>
{
        var result = await service.CreateAttendeeAsync(dto, ct);
        return Results.Created($"/api/attendees/{result.Id}", result);
});

attendees.MapPut("/{id:guid}", async (Guid id, UpdateAttendeeDTO dto, IAttendeeService service, CancellationToken ct)
    =>
{
    if (id != dto.Id) return Results.BadRequest("Id mismatch");

    var updatedAttendee = await service.UpdateAttendeeAsync(dto, ct);
    return Results.Ok(updatedAttendee);
});

attendees.MapDelete("/{id:guid}", async (Guid id, IAttendeeService service, CancellationToken ct) =>
{
    await service.DeleteAttendeeAsync(id, ct);
    return Results.NoContent();
});

attendees.MapPost("/{id:guid}/competences", async (Guid id, AddCompetenceRequest request, IAttendeeService service, CancellationToken ct) =>
{
    await service.AddCompetenceToInstructorAsync(id, request.CompetenceName, request.RowVersion, ct);
    return Results.Ok(new { message = $"Competence '{request.CompetenceName}' added successfully." });
});


#endregion

#region Competences

var competences = app.MapGroup("/api/competences");

competences.MapGet("/", async (ICompetenceService service, CancellationToken ct) =>
Results.Ok(await service.GetAllCompetencesAsync(ct)));

competences.MapGet("/{id:guid}", async (Guid id, ICompetenceService service, CancellationToken ct) =>
Results.Ok(await service.GetCompetenceDetailsAsync(id, ct)));

competences.MapPost("/", async (CreateCompetenceDTO dto, ICompetenceService service, CancellationToken ct) =>
{
    var result = await service.CreateCompetenceAsync(dto, ct);
    return Results.Created($"/api/competences/{result.Id}", result);
});

competences.MapPut("/{id:guid}", async (Guid id, UpdateCompetenceDTO dto, ICompetenceService service, CancellationToken ct) =>
{
    if (id != dto.Id) return Results.BadRequest("Id mismatch");
    var updateCompetence = await service.UpdateCompetenceAsync(dto, ct);
    return Results.Ok(updateCompetence);
});

competences.MapDelete("/{id:guid}", async (Guid id, ICompetenceService service, CancellationToken ct) =>
{
    await service.DeleteCompetenceAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region Courses

var courses = app.MapGroup("/api/courses");

courses.MapGet("/", async (ICourseService service, CancellationToken ct) =>
    Results.Ok(await service.GetAllCoursesAsync(ct)));

courses.MapGet("/{name}", async (string name, ICourseService service, CancellationToken ct)
    => Results.Ok(await service.GetCourseByNameAsync(name, ct)));

courses.MapPost("/", async (CreateCourseDTO dto, ICourseService service, CancellationToken ct) =>
{
        var result = await service.CreateCourseAsync(dto, ct);
        return Results.Created($"/api/courses/{result.Id}", result);
});

#endregion

#region Location



#endregion

#region CourseSessions



#endregion

app.Run();

