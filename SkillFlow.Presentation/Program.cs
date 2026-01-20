using Microsoft.EntityFrameworkCore;
using SkillFlow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<SkillFlowDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(""));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();

