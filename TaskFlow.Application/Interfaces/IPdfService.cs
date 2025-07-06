using TaskFlow.Application.Dtos.Response;

namespace TaskFlow.Application.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateTasksOverviewPdf(IEnumerable<ScheduledTaskDto> tasks);

    }
}
