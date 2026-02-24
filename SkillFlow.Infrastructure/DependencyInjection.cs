using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using SkillFlow.Application.Abstractions.Caching;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Infrastructure.Caching;
using SkillFlow.Infrastructure.Persistence;
using SkillFlow.Infrastructure.Repositories;

namespace SkillFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SkillFlowDbContext>(options => options.UseSqlServer(connectionString));

            services.AddMemoryCache();

            services.AddSingleton<IAttendeeCacheBuster, MemoryAttendeeCacheBuster>();

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddScoped<AttendeeRepository>();

            services.AddScoped<IAttendeeRepository>(sp =>
            new CachedAttendeeRepository(
                sp.GetRequiredService<AttendeeRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<IAttendeeCacheBuster>()));

            services.AddScoped<IAttendeeQueries>(sp =>
            new CachedAttendeeQueries(
                sp.GetRequiredService<AttendeeRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<IAttendeeCacheBuster>()));

            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ICourseSessionRepository, CourseSessionRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ICompetenceRepository, CompetenceRepository>();

            return services;
        }
    }
}
