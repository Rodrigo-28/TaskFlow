namespace TaskFlow.Application.Interfaces
{
    public interface IEmailService
    {
        public Task SendConfirmationEmailAsync(string toEmail, string customerName, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
