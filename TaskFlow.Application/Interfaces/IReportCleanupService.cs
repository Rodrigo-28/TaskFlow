namespace TaskFlow.Application.Interfaces
{
    public interface IReportCleanupService
    {
        Task<int> CleanOldReportFilesAsync(int? days);
    }
}
