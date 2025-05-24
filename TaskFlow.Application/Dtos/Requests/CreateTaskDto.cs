using TaskFlow.Domain.Enums;

namespace TaskFlow.Application.Dtos.Requests
{
    public class CreateTaskDto
    {
        public string Name { get; set; }
        public TaskType Type { get; set; }
        public string? CronExpression { get; set; }
        public DateTimeOffset? ScheduledTime { get; set; }
        // ——— Estos sólo se usan cuando Type == Email ———
        public string? ToEmail { get; set; }
        public string? CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
