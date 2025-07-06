using TaskFlow.Application.Dtos.Response;

namespace TaskFlow.Application.Interfaces
{
    public interface IReportService
    {
        Task<ReportFileDto> GetLatestOverviewReportAsync();
    }
}
