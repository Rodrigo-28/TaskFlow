using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Domain.Models;
using TaskFlow.Infrastructure.Contexts;

namespace TaskFlow.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            this._context = context;
        }
        public async Task<TaskExecutionLog> AddExecutionLogAsync(TaskExecutionLog log)
        {
            _context.TaskExecutionLogs.Add(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<ScheduledTask> Create(ScheduledTask task)
        {
            _context.ScheduledTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;

        }

        public async Task<bool> Delete(ScheduledTask task)
        {
            _context.ScheduledTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ScheduledTask>> GetAll()
        {
            return await _context.ScheduledTasks.ToListAsync();
        }

        public async Task<ScheduledTask?> GetById(int id)
        {
            return await _context.ScheduledTasks
                                    .Include(t => t.ExecutionLogs)
                                    .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskExecutionLog>> GetExecutionLogsAsync(int taskId)
        {
            return await _context.TaskExecutionLogs
                                .Where(l => l.ScheduledTaskId == taskId)
                                .ToListAsync();

        }

        public async Task<ScheduledTask> Update(ScheduledTask task)
        {
            _context.ScheduledTasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }
    }
}
