using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Application.Dtos.Requests
{
    public class UpdateDataCleanupTaskDto : UpdateTaskDto
    {
        [Range(1, 365, ErrorMessage = "LogRetentionDays debe ser al menos 1 día")]
        public int? LogRetentionDays { get; set; }

        [Range(1, 365, ErrorMessage = "FileRetentionDays debe ser al menos 1 día")]
        public int? FileRetentionDays { get; set; }
    }
}
