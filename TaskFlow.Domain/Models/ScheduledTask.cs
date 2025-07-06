namespace TaskFlow.Domain.Models
{
    public class ScheduledTask
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string? CronExpression { get; set; }    // for recurring tasks
        public DateTimeOffset? ScheduledTime { get; set; } // for one-off tasks
        public bool IsActive { get; set; } = true;
        public DateTimeOffset? LastRunTime { get; set; }


        public ICollection<TaskExecutionLog> ExecutionLogs { get; set; } = new List<TaskExecutionLog>();
    }
}
