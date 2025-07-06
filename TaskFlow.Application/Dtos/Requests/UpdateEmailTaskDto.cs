namespace TaskFlow.Application.Dtos.Requests
{
    public class UpdateEmailTaskDto : UpdateTaskDto
    {

        public string? ToEmail { get; set; }

        public string? CustomerName { get; set; }

        public DateTimeOffset? StartDate { get; set; }

        public DateTimeOffset? EndDate { get; set; }
    }
}
