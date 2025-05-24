namespace TaskFlow.Application.Interfaces
{
    public interface IJobScheduler
    {
        void ScheduleRecurringTask(int taskId, string cronExpression);
        void ScheduleDelayedTask(int taskId, DateTime scheduledTime);
        void ScheduleImmediateTask(int taskId);
        void RemoveRecurringTask(int taskId);
    }
}
