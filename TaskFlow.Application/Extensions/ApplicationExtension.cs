using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Interfaces;
using TaskFlow.Application.Services;

namespace TaskFlow.Application.Extensions
{
    public static class ApplicationExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            {


                services.AddTransient<ITaskService, TaskService>();
                services.AddTransient<IEmailService, EmailService>();





                return services;
            }
        }
    }
}
