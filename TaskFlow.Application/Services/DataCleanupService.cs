using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Common;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Services
{
    public class DataCleanupService : IDataCleanupService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly CleanupOptions _opts;

        public DataCleanupService(ITaskRepository taskRepository, IOptions<CleanupOptions> optsAccessor)
        {
            this._taskRepository = taskRepository;
            _opts = optsAccessor.Value;
        }
        public async Task CleanOldExecutionLogsAsync(int? days)
        {
            var retention = days ?? _opts.DefaultLogRetentionDays;

            if (retention < 1)
                throw new ValidationException("Retention days debe ser al menos 1.");

            var cutOff = DateTime.UtcNow.AddDays(-retention);
            var deletedCount = await _taskRepository.DeleteLogsOlderThanAsync(cutOff);
            Console.WriteLine($"[DataCleanup] Borrados {deletedCount} logs mayores de {retention} días.");
        }
    }
}
