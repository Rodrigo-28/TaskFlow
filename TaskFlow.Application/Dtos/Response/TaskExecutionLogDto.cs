namespace TaskFlow.Application.Dtos.Response
{
    public class TaskExecutionLogDto
    {
        public int Id { get; set; }
        public int ScheduledTaskId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
