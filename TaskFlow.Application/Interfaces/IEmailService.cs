namespace TaskFlow.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendConfirmationEmailAsync(string toEmail, string customerName, DateTime startDate, DateTime endDate);
    }
}
