namespace TaskFlow.Application.Interfaces
{
    public interface IDataCleanupService
    {
        Task CleanOldExecutionLogsAsync(int? days);
    }
}
