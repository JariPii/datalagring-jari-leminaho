using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SkillFlow.Application;
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
using SkillFlow.Presentation.Endpoints;
using SkillFlow.Presentation.Exceptions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var enumConverter = new JsonStringEnumConverter();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(enumConverter);
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(enumConverter);
});

builder.Services.AddApplication();

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
        policy.WithOrigins("https://localhost:3000",
            "http://localhost:3000",
            "https://127.0.0.1:3000",
            "http://127.0.0.1:3000")
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

app.UseHttpsRedirection();

app.UseCors("NextJsPolicy");

app.MapAttenteesEnpoints();
app.MapCoursesEndpoints();
app.MapCompetencesEndpoints();
app.MapLocationsEndpoints();
app.MapCourseSessionsEndpoints();

app.Run();

