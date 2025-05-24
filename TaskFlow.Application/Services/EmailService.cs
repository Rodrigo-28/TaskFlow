using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public EmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this._httpClient = httpClientFactory.CreateClient("Mailgun");
            this._configuration = configuration;
        }
        public async Task SendConfirmationEmailAsync(string toEmail, string customerName, DateTime startDate, DateTime endDate)
        {
            var formattedStarDate = startDate.ToLocalTime().ToString("dd/MM/yyyy");
            var formattedEndDate = endDate.ToLocalTime().ToString("dd/MM/yyyy");

            var text = $"Hola {customerName}, tu task está confirmado empieza el {formattedStarDate} hasta el {formattedEndDate}.";
            var html = $@"
        <html>
            <body>
                <h1>¡task confirmada!</h1>
                <p>Estimado {customerName},</p>
                <p>Tu auto estará disponible desde el <strong>{formattedStarDate}</strong> hasta el <strong>{formattedEndDate}</strong>.</p>
            </body>
        </html>
             ";

            await SendEmailAsync(toEmail, "task confirmation", text, html);

        }

        private async Task SendEmailAsync(string toEmail, string subject, string textContent, string htmlContent)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                var domain = _configuration["Mailgun:Domain"];
                AddFormParam(form, "to", toEmail);
                AddFormParam(form, "subject", subject);
                AddFormParam(form, "text", textContent);
                AddFormParam(form, "html", htmlContent);
                _httpClient.DefaultRequestHeaders.Accept.Add
                 (new MediaTypeWithQualityHeaderValue("multipart/form-data"));
                // envio solicitud
                var response = await _httpClient.PostAsync("", form);

                if (!response.IsSuccessStatusCode)
                {
                    var erroContent = await response.Content.ReadAsStringAsync();
                    var errorMessage = $"Mailgun Error: {response.StatusCode} ";
                    throw new Exception(errorMessage);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error completo: {ex.ToString()}");
                throw new Exception("No se pudo enviar el correo de confirmación", ex);
            }
        }


        private void AddFormParam(MultipartFormDataContent form, string key, string value)
        {
            form.Add(new StringContent(value, Encoding.UTF8, MediaTypeNames.Text.Plain), key);
        }
    }
}
