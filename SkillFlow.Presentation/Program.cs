using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Competences;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.DTOs.CourseSessions;
using SkillFlow.Application.DTOs.Locations;
using SkillFlow.Application.Interfaces;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Application.Services.Competences;
using SkillFlow.Application.Services.Courses;
using SkillFlow.Application.Services.CourseSessions;
using SkillFlow.Application.Services.Locations;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Persistence;
using SkillFlow.Infrastructure.Repositories;
using SkillFlow.Presentation.Exceptions;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddDbContext<SkillFlowDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

#region AddScoped

builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();
builder.Services.AddScoped<IAttendeeQueries, AttendeeRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseSessionRepository, CourseSessionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<ICompetenceRepository, CompetenceRepository>();

builder.Services.AddScoped<IAttendeeService, AttendeeService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseSessionService, CourseSessionService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ICompetenceService, CompetenceService>();

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

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

courses.MapGet("/search", async (string searchTerm, ICourseService service, CancellationToken ct)
    => Results.Ok(await service.SearchCoursesAsync(searchTerm, ct)));

courses.MapPost("/", async (CreateCourseDTO dto, ICourseService service, CancellationToken ct) =>
{
        var result = await service.CreateCourseAsync(dto, ct);
        return Results.Created($"/api/courses/{result.Id}", result);
});

courses.MapPut("/{id:guid}", async (Guid id, UpdateCourseDTO dto, ICourseService service, CancellationToken ct) =>
{
    if (id != dto.Id) return Results.BadRequest("Id mismatch");
    var updateCourse = await service.UpdateCourseAsync(dto, ct);
    return Results.Ok(updateCourse);
});

courses.MapDelete("/{id:guid}", async (Guid id, ICourseService service, CancellationToken ct) =>
{
    await service.DeleteCourseAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region Location

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
});

locations.MapPut("/{id:guid}", async (Guid id, UpdateLocationDTO dto, ILocationService service, CancellationToken ct) =>
{
    if (id != dto.Id) return Results.BadRequest("Id mismatch");
    var updateLocation = await service.UpdateLocationAsync(dto, ct);
    return Results.Ok(updateLocation);
});

locations.MapDelete("/{id:guid}", async (Guid id, ILocationService service, CancellationToken ct) =>
{
    await service.DeleteLocationAsync(id, ct);
    return Results.NoContent();
});

#endregion

#region CourseSessions

var courseSessions = app.MapGroup("/api/courseSessions");

courseSessions.MapGet("/", async (ICourseSessionService service, CancellationToken ct) =>
    Results.Ok(await service.GetAllCourseSessionsAsync(ct)));

courseSessions.MapGet("/{id:guid}", async (Guid id,ICourseSessionService service, CancellationToken ct) =>
    Results.Ok(await service.GetCourseSessionByIdAsync(id, ct)));

courseSessions.MapGet("/available", async (ICourseSessionService service, CancellationToken ct) =>
    Results.Ok(await service.GetAvailableCourseSessionsAsync(ct)));

courseSessions.MapGet("/search", async (string searchTerm, ICourseSessionService service, CancellationToken ct) =>
    Results.Ok(await service.SearchCourseSessionsAsync(searchTerm, ct)));

courseSessions.MapGet("/date/{date:datetime}", async (DateTime date, ICourseSessionService service, CancellationToken ct) => 
    Results.Ok(await service.GetCourseSessionsByDateAsync(date, ct)));

courseSessions.MapGet("/location/{locationId:guid}", async(Guid locationId, ICourseSessionService service, CancellationToken ct) =>
    Results.Ok(await service.GetCourseSessionsByLocationAsync(locationId, ct)));

courseSessions.MapPost("/", async (CreateCourseSessionDTO dto, ICourseSessionService service, CancellationToken ct) =>
{
    var result = await service.CreateCourseSessionAsync(dto, ct);
    return Results.Created($"/api/courseSessions/{result.Id}", result);
});

courseSessions.MapPut("/{id:guid}", async (Guid id, UpdateCourseSessionDTO dto, ICourseSessionService service, CancellationToken ct) =>
{
    if (id != dto.Id) return Results.BadRequest("Id mismatch");

    var updated = await service.UpdateCourseSessionAsync(dto, ct);
    return Results.Ok(updated);
});

courseSessions.MapDelete("/{id:guid}", async (Guid id, ICourseSessionService service, CancellationToken ct) =>
{
    await service.DeleteCourseSessionAsync(id, ct);
    return Results.NoContent();
});

courseSessions.MapPost("/{id:guid}/instructors", async (Guid id, AddInstructorToCourseSessionDTO dto, ICourseSessionService service, CancellationToken ct) =>
{
    await service.AddInstructorToCourseSessionAsync(id, dto.InstructorId, dto.RowVersion, ct);
    return Results.Ok();
});

courseSessions.MapPost("/{id:guid}/enrollments", async(Guid id, EnrollStudentDTO dto, ICourseSessionService service, CancellationToken ct) =>
{
    await service.EnrollStudentAsync(id, dto.StudentId, ct);
    return Results.Ok();
});

courseSessions.MapGet("/{id:guid}/enrollments", async (Guid id, ICourseSessionService service, CancellationToken ct) => Results.Ok(await service.GetEnrollmentsBySessionIdAsync(id, ct)));

courseSessions.MapPut("/{id:guid}/enrollment/{studentId:guid}/status", async (Guid id, Guid studentId, UpdateEnrollmentStatusDTO dto, ICourseSessionService service, CancellationToken ct) =>
{
    await service.SetEnrollmentStatusAsync(id, studentId, dto.NewStatus, dto.RowVersion, ct);
    return Results.Ok();
});
#endregion

app.Run();

