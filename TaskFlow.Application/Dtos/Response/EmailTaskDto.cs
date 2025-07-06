namespace TaskFlow.Application.Dtos.Response
{
    public class EmailTaskDto : ScheduledTaskDto
    {
        public string ToEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
