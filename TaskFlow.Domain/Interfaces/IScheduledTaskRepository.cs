using TaskFlow.Domain.Models;

namespace TaskFlow.Domain.Interfaces
{
    public interface IScheduledTaskRepository
    {
        Task<ScheduledTask?> GetById(int id);
        Task<IEnumerable<ScheduledTask>> GetAll();
        Task<ScheduledTask> Create(ScheduledTask task);
        Task<ScheduledTask> Update(ScheduledTask task);
        Task<bool> Delete(int id);
    }
}
