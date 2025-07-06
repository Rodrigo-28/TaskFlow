using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Services;

namespace TaskFlow.Infrastructure.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //repositorios
            services.AddTransient<ITaskRepository, TaskRepository>();

            //MailgunServices
            services.AddMailgunServices(configuration);

            //ReportService
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportFileRepository, ReportFileRepository>();




            return services;
        }
    }
}
