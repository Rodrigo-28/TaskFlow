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

        public static IServiceCollection AddHangfireServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHangfire(hf => hf
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddHangfireServer();

            services.AddSingleton<IJobScheduler, HangfireJobScheduler>();

            return services;
        }
    }
}
