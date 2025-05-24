using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Dtos.Response
{
    public class ScheduledTaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TaskType Type { get; set; }
        public string? CronExpression { get; set; }
        public DateTimeOffset? ScheduledTime { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset? LastRunTime { get; set; }
        public IEnumerable<TaskExecutionLogDto> ExecutionLogs { get; set; }
    }
}
