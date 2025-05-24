using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Interfaces;
using TaskFlow.Infrastructure.Scheduling;

namespace TaskFlow.Infrastructure.Extensions
{
    public static class HangfireExtension
    {
        /// <summary>
        /// Adds and configures Hangfire services for background job scheduling.
        /// Call this in Program.cs of your API project.
        /// </summary>
        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Configure Hangfire to use PostgreSQL as storage
            services.AddHangfire(hf => hf
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"))
            );

            // 2. Register the Hangfire server to process jobs
            services.AddHangfireServer();

            // 3. Register the scheduling abstraction implementation
            services.AddSingleton<IJobScheduler, HangfireJobScheduler>();

            return services;
        }
    }
}
