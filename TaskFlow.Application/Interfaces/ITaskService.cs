using TaskFlow.Application.Dtos.Requests;
using TaskFlow.Application.Dtos.Response;

namespace TaskFlow.Application.Interfaces
{
    public interface ITaskService
    {
        Task<ScheduledTaskDto> CreateTask(CreateTaskDto dto);
        Task<ScheduledTaskDto?> GetTask(int id);
        Task<IEnumerable<ScheduledTaskDto>> GetAllTasks();
        Task<ScheduledTaskDto> UpdateTask(int id, UpdateTaskDto dto);
        Task<bool> DeleteTask(int id);
        Task ExecuteTaskAsync(int taskId);

    }
}
