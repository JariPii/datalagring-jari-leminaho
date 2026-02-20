using Microsoft.AspNetCore.Http.Json;
using SkillFlow.Presentation.Exceptions;
using System.Text.Json.Serialization;

namespace SkillFlow.Presentation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddOpenApi();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            var enumConverter = new JsonStringEnumConverter();

            services.Configure<JsonOptions>(options =>
            {
                options.SerializerOptions.Converters.Add(enumConverter);
            });

            services.AddCors(options =>
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

            return services;
        }
    }
}
