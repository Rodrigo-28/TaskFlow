using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Domain.Common
{
    public class CleanupOptions
    {
        [Required]
        public string ReportsFolder { get; set; } = null!;
        [Range(1, int.MaxValue)]
        public int DefaultLogRetentionDays { get; set; }
        [Range(1, int.MaxValue)]
        public int DefaultFileRetentionDays { get; set; }
    }
}
