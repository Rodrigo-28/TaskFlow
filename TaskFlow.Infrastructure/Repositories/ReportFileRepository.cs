using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Infrastructure.Repositories
{
    public class ReportFileRepository : IReportFileRepository
    {
        public Task<int> DeleteFilesOlderThanAsync(string folder, DateTime cutoffUtc)
        {
            var dir = new DirectoryInfo(folder);
            var files = dir.Exists
                ? dir.EnumerateFiles("TasksOverview_*.pdf")
                     .Where(f => f.CreationTimeUtc < cutoffUtc)
                     .ToList()
                : new List<FileInfo>();

            foreach (var f in files)
            {
                f.Delete();
            }
                    ;
            return Task.FromResult(files.Count);
        }
    }
}
