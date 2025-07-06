using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services
{
    public class ReportCleanupService : IReportCleanupService
    {
        private readonly IReportFileRepository _reportFileRepository;

        private readonly CleanupOptions _opts;

        public ReportCleanupService(IReportFileRepository reportFileRepository, IOptions<CleanupOptions> optsAccessor)
        {
            this._reportFileRepository = reportFileRepository;

            _opts = optsAccessor.Value;

        }
        public async Task<int> CleanOldReportFilesAsync(int? days)
        {
            var retention = days ?? _opts.DefaultFileRetentionDays;
            if (retention < 1)
                throw new ValidationException("Retention days debe ser al menos 1.");

            var cutoff = DateTime.UtcNow.AddDays(-retention);

            if (!Directory.Exists(_opts.ReportsFolder))
                throw new DirectoryNotFoundException($"Carpeta de reportes no encontrada: {_opts.ReportsFolder}");


            int deletedCount = await _reportFileRepository.DeleteFilesOlderThanAsync(_opts.ReportsFolder, cutoff);
            return deletedCount;
        }
    }
}
