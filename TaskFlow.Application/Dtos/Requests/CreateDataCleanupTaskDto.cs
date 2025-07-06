using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Dtos.Requests
{
    public class CreateDataCleanupTaskDto : CreateScheduledTaskDto
    {
        [Range(1, 365, ErrorMessage = "Debe ser al menos 1 día")]
        public int LogRetentionDays { get; set; }

        [Range(1, 365, ErrorMessage = "Debe ser al menos 1 día")]
        public int FileRetentionDays { get; set; }
    }
}
