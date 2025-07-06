namespace TaskFlow.Application.Dtos.Requests
{
    public class UpdateTaskDto
    {
        public string? Name { get; set; }

        public string? CronExpression { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
