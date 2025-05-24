using System.ComponentModel.DataAnnotations;
using TaskFlow.Application.Dtos.Requests;
using TaskFlow.Application.Dtos.Response;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IJobScheduler _jobScheduler;
        private readonly IEmailService _emailService;

        public TaskService(ITaskRepository taskRepository, IJobScheduler jobScheduler, IEmailService emailService)
        {
            this._taskRepository = taskRepository;
            this._jobScheduler = jobScheduler;
            this._emailService = emailService;
        }

        public async Task<ScheduledTaskDto> CreateTask(CreateTaskDto dto)
        {
            var task = new ScheduledTask()
            {
                Name = dto.Name,
                Type = dto.Type,
                CronExpression = dto.CronExpression,
                ScheduledTime = dto.ScheduledTime?.ToUniversalTime(),
                ToEmail = dto.ToEmail,
                CustomerName = dto.CustomerName,
                StartDate = dto.StartDate?.ToUniversalTime(),
                EndDate = dto.EndDate?.ToUniversalTime(),
                IsActive = true
            };
            if (task.ScheduledTime.HasValue && task.ScheduledTime.Value.UtcDateTime <= DateTime.UtcNow)
            {
                throw new ValidationException("scheduledTime must be in the future");
            }
            var created = await _taskRepository.Create(task);
            // Schedule job based on its configuration
            ScheduleTask(created);





            return ToDto(created);
        }

        public async Task<bool> DeleteTask(int id)
        {
            var existing = await _taskRepository.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException($"Task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            }
            //remove any recurring schedule
            _jobScheduler.RemoveRecurringTask(id);
            return await _taskRepository.Delete(existing);

        }

        public async Task ExecuteTaskAsync(int taskId)
        {
            var start = DateTime.UtcNow;
            var task = await _taskRepository.GetById(taskId);

            try
            {
                switch (task.Type)
                {
                    case TaskType.Email:
                        await _emailService.SendConfirmationEmailAsync(
                            task.ToEmail,
                            task.CustomerName,
                            task.StartDate!.Value,
                            task.EndDate!.Value


                            );
                        break;
                    case TaskType.PDFReport:
                        Console.WriteLine("Envio de pdf");
                        break;
                    case TaskType.DataCleanup:
                        Console.WriteLine("Limpieza de datos");
                        break;
                    default:
                        throw new NotSupportedException($"Task type '{task.Type}' is not supported.");
                }
                task.LastRunTime = DateTime.UtcNow;
                await _taskRepository.Update(task);

                //log success
                await _taskRepository.AddExecutionLogAsync(new TaskExecutionLog()
                {
                    ScheduledTaskId = taskId,
                    StartTime = start,
                    EndTime = DateTime.UtcNow,
                    Success = true
                });

            }
            catch (Exception ex)
            {
                //Log failure
                await _taskRepository.AddExecutionLogAsync(new TaskExecutionLog
                {
                    ScheduledTaskId = taskId,
                    StartTime = start,
                    EndTime = DateTime.UtcNow,
                    Success = false,
                    ErrorMessage = ex.Message
                });
                throw;
            }

        }

        public async Task<IEnumerable<ScheduledTaskDto>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAll();
            return tasks.Select(ToDto);
        }

        public async Task<ScheduledTaskDto?> GetTask(int id)
        {
            var task = await _taskRepository.GetById(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            }
            return ToDto(task);

        }

        public async Task<ScheduledTaskDto> UpdateTask(int id, UpdateTaskDto dto)
        {
            var existing = await _taskRepository.GetById(id);
            if (existing == null)
            {
                throw new NotFoundException($"Task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            }
            existing.Name = dto.Name ?? existing.Name;
            if (dto.Type.HasValue)
            {
                existing.Type = dto.Type.Value;
            };
            if (dto.CronExpression != null)
                existing.CronExpression = dto.CronExpression;

            if (dto.ScheduledTime.HasValue)
                existing.ScheduledTime = dto.ScheduledTime.Value.ToUniversalTime();
            if (dto.IsActive.HasValue)
                existing.IsActive = dto.IsActive.Value;
            if (existing.ScheduledTime.HasValue && existing.ScheduledTime.Value.UtcDateTime <= DateTime.UtcNow)
            {
                throw new ValidationException("scheduledTime must be in the future");
            }
            await _taskRepository.Update(existing);
            // Reset schedule
            _jobScheduler.RemoveRecurringTask(id);
            ScheduleTask(existing);
            return ToDto(existing);

        }

        private void ScheduleTask(ScheduledTask task)
        {
            if (!task.IsActive) return;

            if (!string.IsNullOrWhiteSpace(task.CronExpression))
            {
                //Cronos.CronExpression.Parse(task.CronExpression);
                _jobScheduler.ScheduleRecurringTask(task.Id, task.CronExpression);

            }
            else if (task.ScheduledTime.HasValue)
            {
                var utc = task.ScheduledTime.Value.UtcDateTime;
                _jobScheduler.ScheduleDelayedTask(task.Id, utc);
            }

        }
        private ScheduledTaskDto ToDto(ScheduledTask task)
        {
            return new ScheduledTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Type = task.Type,
                CronExpression = task.CronExpression,
                ScheduledTime = task.ScheduledTime,
                IsActive = task.IsActive,
                LastRunTime = task.LastRunTime,
                ExecutionLogs = task.ExecutionLogs.Select(log => new TaskExecutionLogDto
                {
                    StartTime = log.StartTime,
                    EndTime = log.EndTime,
                    Success = log.Success,
                    ErrorMessage = log.ErrorMessage
                }).ToList()
            };
        }

    }
}
