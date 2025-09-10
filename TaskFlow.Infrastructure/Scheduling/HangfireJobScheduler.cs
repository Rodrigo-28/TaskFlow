using Hangfire;
using TaskFlow.Application.Interfaces;

namespace TaskFlow.Infrastructure.Scheduling
{



    public class HangfireJobScheduler : IJobScheduler
    {
        public void RemoveRecurringTask(int taskId)
        {
            //Tipo de operación: Cancelación de un job recurrente.
            RecurringJob.RemoveIfExists($"task-{taskId}");
        }

        public void ScheduleDelayedTask(int taskId, DateTime scheduledTime)
        {
            //Tipo de job: Delayed(diferido, se ejecuta una única vez en el momento que indiques).
            BackgroundJob.Schedule<ITaskService>(
                service => service.ExecuteTaskAsync(taskId),
                scheduledTime);
        }

        public void ScheduleImmediateTask(int taskId)
        {
            //Tipo de job: Fire - and - forget(se ejecuta una sola vez, lo antes posible).
            BackgroundJob.Enqueue<ITaskService>(
                service => service.ExecuteTaskAsync(taskId));

        }

        public void ScheduleRecurringTask(int taskId, string cronExpression)
        {
            //Tipo de job: Recurrente(se ejecuta una y otra vez siguiendo un patrón cron).
            RecurringJob.AddOrUpdate<ITaskService>(
                $"task-{taskId}",
                service => service.ExecuteTaskAsync(taskId),
                cronExpression
            );
        }
    }

}
