using TaskFlow.Application.Dtos.Requests;
using TaskFlow.Application.Dtos.Response;

namespace TaskFlow.Application.Interfaces
{
    public interface ITaskService
    {
        Task<EmailTaskDto> CreateEmailTaskAsync(CreateEmailTaskDto dto);
        Task<PdfReportTaskDto> CreatePdfReportTaskAsync(CreatePdfReportTaskDto dto);
        Task<DataCleanupTaskDto> CreateDataCleanupTaskAsync(CreateDataCleanupTaskDto dto);

        Task<IEnumerable<ScheduledTaskDto>> GetAllTasksAsync();
        Task<ScheduledTaskDto> GetTaskByIdAsync(int id);

        Task<EmailTaskDto> UpdateEmailTaskAsync(int id, UpdateEmailTaskDto dto);
        Task<PdfReportTaskDto> UpdatePdfReportTaskAsync(int id, UpdatePdfReportTaskDto dto);
        Task UpdateDataCleanupTaskAsync(int id, UpdateDataCleanupTaskDto dto);
        Task DeleteTaskAsync(int id);
        Task ExecuteTaskAsync(int id);
    }
}
