namespace TaskFlow.Application.Dtos.Response
{
    public class DataCleanupTaskDto : ScheduledTaskDto
    {
        public int LogRetentionDays { get; set; }
        public int FileRetentionDays { get; set; }
    }
}
