using Microsoft.AspNetCore.Hosting;
using TaskFlow.Application.Dtos.Response;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Services
{



    public class ReportService : IReportService
    {
        private readonly string _reportsFolder;
        public ReportService(IWebHostEnvironment env)
        {
            _reportsFolder = Path.Combine(env.ContentRootPath, "Reports");
        }
        public async Task<ReportFileDto> GetLatestOverviewReportAsync()
        {
            var dir = new DirectoryInfo(_reportsFolder);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Carpeta no encontrada: {_reportsFolder}");
            }
            var latest = dir
                .EnumerateFiles("TasksOverview_*.pdf")
                .OrderByDescending(f => f.CreationTimeUtc)
                .FirstOrDefault();
            if (latest is null)
                throw new FileNotFoundException("No se encontró ningún PDF de overview.");
            var content = await File.ReadAllBytesAsync(latest.FullName);
            return new ReportFileDto
            {
                Content = content,
                FileName = latest.FullName,
            };
        }
    }
}
