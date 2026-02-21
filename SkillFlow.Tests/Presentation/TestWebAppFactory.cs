using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using SkillFlow.Application.Interfaces;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Application.Services.CourseSessions;

namespace SkillFlow.Tests.Presentation;

public class TestWebAppFactory : WebApplicationFactory<Program>
{
    public Mock<IAttendeeService> AttendeeServiceMock { get; } = new();
    public Mock<ICourseSessionService> CourseSessionServiceMock { get; } = new();
    public Mock<ICompetenceService> CompetenceServiceMock { get; } = new();
    public Mock<ICourseService> CourseServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IAttendeeService));
            services.AddSingleton(AttendeeServiceMock.Object);

            services.RemoveAll(typeof(ICourseSessionService));
            services.AddSingleton(CourseSessionServiceMock.Object);

            services.RemoveAll(typeof(ICompetenceService));
            services.AddSingleton(CompetenceServiceMock.Object);

            services.RemoveAll(typeof(ICourseService));
            services.AddSingleton(CourseServiceMock.Object);
        });
    }
}