using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure.Persistence;
using SkillFlow.Infrastructure.Repositories;

namespace SkillFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SkillFlowDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddScoped<IAttendeeRepository, AttendeeRepository>();
            services.AddScoped<IAttendeeQueries, AttendeeRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseSessionRepository, CourseSessionRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ICompetenceRepository, CompetenceRepository>();

            return services;
        }
    }
}
