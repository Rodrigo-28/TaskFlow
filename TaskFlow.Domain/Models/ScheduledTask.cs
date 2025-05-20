using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Models
{
    public class ScheduledTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public TaskType Type { get; set; }
        public string? CronExpression { get; set; }    // for recurring tasks
        public DateTime? ScheduledTime { get; set; } // for one-off tasks
        public bool IsActive { get; set; } = true;
        public DateTime? LastRunTime { get; set; }
        public ICollection<TaskExecutionLog> ExecutionLogs { get; set; } = new List<TaskExecutionLog>();
    }
}
