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

            services.AddSingleton<IAttendeeCacheBuster, AttendeeCacheBuster>();
            services.AddSingleton<ICourseCacheBuster, CourseCacheBuster>();
            services.AddSingleton<ILocationCacheBuster, LocationCacheBuster>();
            services.AddSingleton<ICompetenceCacheBuster, CompetenceCacheBuster>();
            services.AddSingleton<ICourseSessionCacheBuster, CourseSessionCacheBuster>();

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

            services.AddScoped<CourseRepository>();
            services.AddScoped<ICourseRepository>(sp =>
            new CachedCourseRepository(
                sp.GetRequiredService<CourseRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ICourseCacheBuster>()));

            services.AddScoped<LocationRepository>();
            services.AddScoped<ILocationRepository>(sp =>
            new CachedLocationRepository(
                sp.GetRequiredService<LocationRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ILocationCacheBuster>()));

            services.AddScoped<CompetenceRepository>();
            services.AddScoped<ICompetenceRepository>(sp =>
            new CachedCompetenceRepository(
                sp.GetRequiredService<CompetenceRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ICompetenceCacheBuster>()));

            services.AddScoped<CourseSessionRepository>();
            services.AddScoped<ICourseSessionRepository>(sp =>
            new CachedCourseSessionRepository(
                sp.GetRequiredService<CourseSessionRepository>(),
                sp.GetRequiredService<IMemoryCache>(),
                sp.GetRequiredService<ICourseSessionCacheBuster>()));

            return services;
        }
    }
}
