using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Dtos.Requests
{
    public class CreateEmailTaskDto : CreateScheduledTaskDto
    {
        [Required, EmailAddress]
        public string ToEmail { get; set; } = null!;

        [Required]
        public string CustomerName { get; set; } = null!;

        [Required]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        public DateTimeOffset EndDate { get; set; }
    }
}
