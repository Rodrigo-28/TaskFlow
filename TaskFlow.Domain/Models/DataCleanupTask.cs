namespace TaskFlow.Domain.Models
{
    public class DataCleanupTask : ScheduledTask
    {
        public int LogRetentionDays { get; set; }
        public int FileRetentionDays { get; set; }
    }
}
