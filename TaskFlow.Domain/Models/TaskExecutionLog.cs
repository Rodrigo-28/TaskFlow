namespace TaskFlow.Domain.Models
{
    public class TaskExecutionLog
    {
        public int Id { get; set; }
        public int ScheduledTaskId { get; set; }
        public ScheduledTask ScheduledTask { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
