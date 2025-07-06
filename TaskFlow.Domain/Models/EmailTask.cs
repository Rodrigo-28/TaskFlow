namespace TaskFlow.Domain.Models
{
    public class EmailTask : ScheduledTask
    {
        public string ToEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
