using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Dtos.Requests
{
    public class UpdateTaskDto
    {
        public string? Name { get; set; }
        public TaskType? Type { get; set; }
        public string? CronExpression { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
