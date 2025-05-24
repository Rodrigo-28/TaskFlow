using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Interfaces
{
    public interface ITaskRepository
    {
        Task<ScheduledTask?> GetById(int id);
        Task<IEnumerable<ScheduledTask>> GetAll();
        Task<ScheduledTask> Create(ScheduledTask task);
        Task<ScheduledTask> Update(ScheduledTask task);
        Task<bool> Delete(ScheduledTask task);
        Task<IEnumerable<TaskExecutionLog>> GetExecutionLogsAsync(int taskId);
        Task<TaskExecutionLog> AddExecutionLogAsync(TaskExecutionLog log);
    }
}
