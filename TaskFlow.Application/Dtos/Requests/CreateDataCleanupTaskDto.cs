namespace TaskFlow.Application.Dtos.Requests
{
    public class CreateDataCleanupTaskDto : CreateScheduledTaskDto
    {

        public int LogRetentionDays { get; set; }


        public int FileRetentionDays { get; set; }
    }
}
