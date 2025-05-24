using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Text;

namespace TaskFlow.Infrastructure.Extensions
{
    public static class MailgunExtensions
    {
        public static IServiceCollection AddMailgunServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("Mailgun", client =>
            {

                var apiKey = configuration["Mailgun:ApiKey"];
                var domain = configuration["Mailgun:Domain"];
                var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));

                client.BaseAddress = new Uri($"https://api.mailgun.net/v3/{domain}/messages");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            });
            return services;
        }
    }
}
