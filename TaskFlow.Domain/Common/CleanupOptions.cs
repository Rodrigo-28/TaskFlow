namespace TaskFlow.Domain.Common
{
    public class CleanupOptions
    {

        public string ReportsFolder { get; set; } = null!;

        public int DefaultLogRetentionDays { get; set; }

        public int DefaultFileRetentionDays { get; set; }
    }
}
