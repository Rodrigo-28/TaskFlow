using System.ComponentModel.DataAnnotations;
using TaskFlow.Application.Dtos.Requests;
using TaskFlow.Application.Dtos.Response;
using TaskFlow.Application.Exceptions;
using TaskFlow.Application.Interfaces;
using TaskFlow.Domain.Interfaces;
using TaskFlow.Domain.Models;

namespace TaskFlow.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IJobScheduler _jobScheduler;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;
        private readonly IDataCleanupService _dataCleanupService;
        private readonly IReportCleanupService _reportCleanupService;

        public TaskService(ITaskRepository taskRepository, IJobScheduler jobScheduler, IEmailService emailService, IPdfService pdfService,
            IDataCleanupService dataCleanupService, IReportCleanupService reportCleanupService)
        {
            this._taskRepository = taskRepository;
            this._jobScheduler = jobScheduler;
            this._emailService = emailService;
            this._pdfService = pdfService;
            this._dataCleanupService = dataCleanupService;
            this._reportCleanupService = reportCleanupService;
        }

        public async Task<DataCleanupTaskDto> CreateDataCleanupTaskAsync(CreateDataCleanupTaskDto dto)
        {

            if (!string.IsNullOrWhiteSpace(dto.CronExpression)
            && dto.ScheduledTime.HasValue)
            {
                throw new ValidationException(
                    "You cannot specify CronExpression and ScheduledTime simultaneously..");
            }

            if (dto.LogRetentionDays < 1)
                throw new ValidationException("LogRetentionDays must be at least 1 day.");
            if (dto.FileRetentionDays < 1)
                throw new ValidationException("FileRetentionDays must be at least 1 day.");
            if (dto.ScheduledTime.HasValue && dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
                throw new ValidationException("scheduledTime must be in the future.");
            var task = new DataCleanupTask
            {
                Name = dto.Name,
                CronExpression = dto.CronExpression,
                ScheduledTime = dto.ScheduledTime?.ToUniversalTime(),
                IsActive = dto.IsActive,
                LogRetentionDays = dto.LogRetentionDays,
                FileRetentionDays = dto.FileRetentionDays
            };
            var createdBase = await _taskRepository.Create(task);
            if (createdBase is not DataCleanupTask created)
                throw new InvalidOperationException($"DataCleanupTask was expected, but EF returned {createdBase.GetType().Name}");

            ScheduleTask(created);
            var result = new DataCleanupTaskDto
            {
                Id = created.Id,
                Name = created.Name,
                CronExpression = created.CronExpression,
                ScheduledTime = created.ScheduledTime,
                IsActive = created.IsActive,
                LastRunTime = created.LastRunTime,
                LogRetentionDays = created.LogRetentionDays,
                FileRetentionDays = created.FileRetentionDays
            };
            return result;

        }

        public async Task<EmailTaskDto> CreateEmailTaskAsync(CreateEmailTaskDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.CronExpression)
              && dto.ScheduledTime.HasValue)
            {
                throw new ValidationException(
                    "You cannot specify CronExpression and ScheduledTime at the same time.");
            }

            // 2) Si llega scheduledTime, debe estar en el futuro
            if (dto.ScheduledTime.HasValue
                && dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
            {
                throw new ValidationException(
                    "ScheduledTime must be a future date.");
            }
            if (dto.EndDate <= dto.StartDate)
                throw new ValidationException("EndDate must be after StartDate");

            var task = new EmailTask
            {
                Name = dto.Name,
                CronExpression = dto.CronExpression,
                ScheduledTime = dto.ScheduledTime?.ToUniversalTime(),
                IsActive = dto.IsActive,
                ToEmail = dto.ToEmail,
                CustomerName = dto.CustomerName,
                StartDate = dto.StartDate.ToUniversalTime(),
                EndDate = dto.EndDate.ToUniversalTime(),
            };

            var createdBase = await _taskRepository.Create(task);
            if (createdBase is not EmailTask created)
            {
                throw new InvalidOperationException(
                  $" Expected EmailTask, but EF returned {createdBase.GetType().Name}"
                );
            }
            ScheduleTask(created);
            var dtoResult = new EmailTaskDto
            {
                Id = created.Id,
                Name = created.Name,
                CronExpression = created.CronExpression,
                ScheduledTime = created.ScheduledTime,
                IsActive = created.IsActive,
                LastRunTime = created.LastRunTime,

                ToEmail = created.ToEmail,
                CustomerName = created.CustomerName,
                StartDate = created.StartDate,
                EndDate = created.EndDate
            };
            return dtoResult;

        }

        public async Task<PdfReportTaskDto> CreatePdfReportTaskAsync(CreatePdfReportTaskDto dto)
        {

            if (!string.IsNullOrWhiteSpace(dto.CronExpression)
       && dto.ScheduledTime.HasValue)
            {
                throw new ValidationException(
                    "You cannot specify CronExpression and ScheduledTime simultaneously..");
            }
            if (dto.ScheduledTime.HasValue &&
            dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
            {
                throw new ValidationException("scheduledTime must be in the future.");
            }
            var task = new PdfReportTask
            {
                Name = dto.Name,
                CronExpression = dto.CronExpression,
                ScheduledTime = dto.ScheduledTime?.ToUniversalTime(),
                IsActive = dto.IsActive

            };
            var createdBase = await _taskRepository.Create(task);


            if (createdBase is not PdfReportTask created)
            {
                throw new InvalidOperationException(
                    $"Expected PdfReportTask, but EF returned {createdBase.GetType().Name}"
                );
            }
            ScheduleTask(created);
            var result = new PdfReportTaskDto
            {
                Id = created.Id,
                Name = created.Name,
                CronExpression = created.CronExpression,
                ScheduledTime = created.ScheduledTime,
                IsActive = created.IsActive,
                LastRunTime = created.LastRunTime
            };

            return result;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var existing = await _taskRepository.GetById(id);
            if (existing == null)
                throw new NotFoundException($"Task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            _jobScheduler.RemoveRecurringTask(id);
            await _taskRepository.Delete(existing);
        }

        public async Task ExecuteTaskAsync(int id)
        {
            var startTime = DateTime.UtcNow;

            var baseTask = await _taskRepository.GetById(id);
            if (baseTask == null)
            {
                throw new NotFoundException($"Task with ID {id} not found") { ErrorCode = "004" };
            }
            try
            {
                switch (baseTask)
                {
                    case EmailTask emailTask:
                        await _emailService.SendConfirmationEmailAsync(
                                           emailTask.ToEmail,
                                           emailTask.CustomerName,
                                           emailTask.StartDate,
                                           emailTask.EndDate

                                       );
                        Console.WriteLine($"email generado,{emailTask.StartDate},{emailTask.EndDate}");
                        break;

                    case PdfReportTask pdfReportTask:
                        var allTask = (await GetAllTasksAsync()).ToList();
                        var pdfBytes = await _pdfService.GenerateTasksOverviewPdf(allTask);
                        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
                        var fileName = $"TasksOverview_{timestamp}.pdf";
                        var reportsDir = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
                        Directory.CreateDirectory(reportsDir);
                        var fullPath = Path.Combine(reportsDir, fileName);
                        await File.WriteAllBytesAsync(fullPath, pdfBytes);
                        Console.WriteLine($"[PDFReport] Generated overview at {fullPath}");
                        break;
                    case DataCleanupTask cleanupTask:
                        await _dataCleanupService.CleanOldExecutionLogsAsync(null);
                        await _reportCleanupService.CleanOldReportFilesAsync(null);
                        Console.WriteLine($"limpieza generada");
                        break;
                    default:
                        throw new NotSupportedException($"Task type '{baseTask.GetType().Name}' is not supported.");
                }

                baseTask.LastRunTime = DateTime.UtcNow;
                await _taskRepository.Update(baseTask);
                await _taskRepository.AddExecutionLogAsync(new TaskExecutionLog
                {
                    ScheduledTaskId = id,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    Success = true
                });
            }
            catch (Exception ex)
            {

                await _taskRepository.AddExecutionLogAsync(new TaskExecutionLog
                {
                    ScheduledTaskId = id,
                    StartTime = startTime,
                    EndTime = DateTime.UtcNow,
                    Success = false,
                    ErrorMessage = ex.Message
                });


                throw;


            }

        }

        public async Task<IEnumerable<ScheduledTaskDto>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAll();

            return tasks.Select(task => ToDto(task)).ToList();

        }

        public async Task<ScheduledTaskDto> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetById(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            }
            var dto = ToDto(task);

            return dto;
        }

        public async Task UpdateDataCleanupTaskAsync(int id, UpdateDataCleanupTaskDto dto)
        {
            var baseTask = await _taskRepository.GetById(id);
            if (baseTask is not DataCleanupTask existing)
                throw new NotFoundException($"DataCleanupTask with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            if (dto.LogRetentionDays < 1)
                throw new ValidationException("LogRetentionDays debe ser al menos 1 día.");
            if (dto.FileRetentionDays < 1)
                throw new ValidationException("FileRetentionDays debe ser al menos 1 día.");
            if (dto.ScheduledTime.HasValue &&
                dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
                throw new ValidationException("ScheduledTime debe estar en el futuro.");
            existing.Name = dto.Name ?? existing.Name;
            existing.CronExpression = dto.CronExpression ?? existing.CronExpression;
            existing.ScheduledTime = dto.ScheduledTime?.ToUniversalTime();
            existing.IsActive = dto.IsActive ?? existing.IsActive;
            existing.LogRetentionDays = dto.LogRetentionDays ?? existing.LogRetentionDays;
            existing.FileRetentionDays = dto.FileRetentionDays ?? existing.FileRetentionDays;

            await _taskRepository.Update(existing);

            _jobScheduler.RemoveRecurringTask(id);
            ScheduleTask(existing);
        }

        public async Task<EmailTaskDto> UpdateEmailTaskAsync(int id, UpdateEmailTaskDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.CronExpression)
       && dto.ScheduledTime.HasValue)
                throw new ValidationException("No puede especificar CronExpression y ScheduledTime juntos.");
            var baseTask = await _taskRepository.GetById(id);
            if (dto.ScheduledTime.HasValue
       && dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
                throw new ValidationException("ScheduledTime debe ser una fecha futura.");

            if (baseTask is not EmailTask existing)
                throw new NotFoundException($"Email task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            if (dto.EndDate <= dto.StartDate)
                throw new ValidationException("EndDate must be later than StartDate.");

            if (dto.ScheduledTime.HasValue
                && dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
            {
                throw new ValidationException("scheduledTime must be in the future.");
            }
            existing.Name = dto.Name ?? existing.Name;
            existing.CronExpression = dto.CronExpression ?? existing.CronExpression;
            existing.ScheduledTime = dto.ScheduledTime?.ToUniversalTime();
            existing.IsActive = dto.IsActive ?? existing.IsActive;

            existing.ToEmail = dto.ToEmail ?? existing.ToEmail;
            existing.CustomerName = dto.CustomerName ?? existing.CustomerName;
            if (dto.StartDate.HasValue)
                existing.StartDate = dto.StartDate.Value.ToUniversalTime();

            if (dto.EndDate.HasValue)
                existing.EndDate = dto.EndDate.Value.ToUniversalTime();

            await _taskRepository.Update(existing);
            _jobScheduler.RemoveRecurringTask(id);
            ScheduleTask(existing);
            return new EmailTaskDto
            {
                Id = existing.Id,
                Name = existing.Name,
                CronExpression = existing.CronExpression,
                ScheduledTime = existing.ScheduledTime,
                IsActive = existing.IsActive,
                LastRunTime = existing.LastRunTime,
                ToEmail = existing.ToEmail,
                CustomerName = existing.CustomerName,
                StartDate = existing.StartDate,
                EndDate = existing.EndDate
            };

        }

        public async Task<PdfReportTaskDto> UpdatePdfReportTaskAsync(int id, UpdatePdfReportTaskDto dto)
        {
            var baseTask = await _taskRepository.GetById(id);
            if (baseTask is not PdfReportTask existing)
                throw new NotFoundException($"PDF report task with ID {id} not found")
                {
                    ErrorCode = "004"
                };
            if (!string.IsNullOrWhiteSpace(dto.CronExpression)
             && dto.ScheduledTime.HasValue)
            {
                throw new ValidationException(
                    "No puedes establecer a la vez CronExpression y ScheduledTime."
                );
            }
            // 3) Validar formato de la cron (lanza excepción si es inválida)


            if (dto.ScheduledTime.HasValue
                && dto.ScheduledTime.Value.ToUniversalTime() <= DateTime.UtcNow)
            {
                throw new ValidationException("scheduledTime debe estar en el futuro.");
            }
            existing.Name = dto.Name ?? existing.Name;
            existing.CronExpression = dto.CronExpression ?? existing.CronExpression;
            existing.ScheduledTime = dto.ScheduledTime?.ToUniversalTime()
                                      ?? existing.ScheduledTime;
            existing.IsActive = dto.IsActive ?? existing.IsActive;

            await _taskRepository.Update(existing);
            _jobScheduler.RemoveRecurringTask(id);
            ScheduleTask(existing);
            return new PdfReportTaskDto
            {
                Id = existing.Id,
                Name = existing.Name,
                CronExpression = existing.CronExpression,
                ScheduledTime = existing.ScheduledTime,
                IsActive = existing.IsActive,
                LastRunTime = existing.LastRunTime
            };
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
                //Type = task.Type,
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
