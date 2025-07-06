namespace TaskFlow.Application.Dtos.Requests
{
    public class CreateScheduledTaskDto
    {
        public string Name { get; set; }

        public string? CronExpression { get; set; }
        public DateTimeOffset? ScheduledTime { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
