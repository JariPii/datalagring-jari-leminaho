using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SkillFlow.Application;
using SkillFlow.Infrastructure;
using SkillFlow.Presentation;
using SkillFlow.Presentation.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString!);

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

