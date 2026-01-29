using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SkillFlow.Application.DTOs.Attendees;
using SkillFlow.Application.DTOs.Courses;
using SkillFlow.Application.Interfaces;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Application.Services.Courses;
using SkillFlow.Application.Services.CourseSessions;
using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Courses;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure;
using SkillFlow.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddOpenApi();

builder.Services.AddDbContext<SkillFlowDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseSessionRepository, CourseSessionRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();

builder.Services.AddScoped<IAttendeeService, AttendeeService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseSessionService, CourseSessionService>();


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

#region Attendees
app.MapGet("/api/attendees", async (IAttendeeService service, CancellationToken ct) =>
{
    var attendees = await service.GetAllAttendeesAsync(ct);
    return Results.Ok(attendees);
});

app.MapGet("/api/attendees/{id:guid}", async (Guid id, IAttendeeService service, CancellationToken ct) =>
{
    return Results.Ok(await service.GetAttendeeByIdAsync(id, ct));
});

app.MapPost("/api/attendees", async (CreateAttendeeDTO dto, IAttendeeService service, CancellationToken ct) =>
{
    try
    {
        var result = await service.CreateAttendeeAsync(dto, ct);
        return Results.Created($"/api/attendees/{result.Id}", result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

#endregion

app.MapPost("/api/attendees/{id:guid}/competences", async (Guid id, AddCompetenceRequest request, IAttendeeService service, CancellationToken ct) =>
{
    await service.AddCompetenceToInstructorAsync(id, request.CompetenceName, ct);
    return Results.Ok(new { message = $"Competence '{request.CompetenceName}' added successfully." });
});

app.MapPost("/api/courses", async (CreateCourseDTO dto, ICourseService service, CancellationToken ct) =>
{
    try
    {
        var result = await service.CreateCourseAsync(dto, ct);
        return Results.Created($"/api/courses/{result.Id}", result);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Hämta alla kurser
app.MapGet("/api/courses", async (ICourseService service, CancellationToken ct) =>
    Results.Ok(await service.GetAllCoursesAsync(ct)));



app.Run();

