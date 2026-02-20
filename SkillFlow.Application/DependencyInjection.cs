using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SkillFlow.Application.Interfaces;
using SkillFlow.Application.Services.Attendees;
using SkillFlow.Application.Services.Competences;
using SkillFlow.Application.Services.Courses;
using SkillFlow.Application.Services.CourseSessions;
using SkillFlow.Application.Services.Locations;

namespace SkillFlow.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddScoped<IAttendeeService, AttendeeService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseSessionService, CourseSessionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ICompetenceService, CompetenceService>();

            return services;
        }
    }
}
