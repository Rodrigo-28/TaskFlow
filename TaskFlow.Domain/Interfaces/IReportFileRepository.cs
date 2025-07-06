namespace TaskFlow.Domain.Interfaces
{
    public interface IReportFileRepository
    {
        Task<int> DeleteFilesOlderThanAsync(string folder, DateTime cutoffUtc);
    }
}
