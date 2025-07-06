using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Models;

namespace TaskFlow.Infrastructure.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ScheduledTask> ScheduledTasks { get; set; }
        public DbSet<TaskExecutionLog> TaskExecutionLogs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduledTask>(entity =>
            {
                entity.ToTable("ScheduledTasks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CronExpression).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
            modelBuilder.Entity<ScheduledTask>()
      .ToTable("ScheduledTasks")
      .HasDiscriminator<string>("TaskType")
      .HasValue<EmailTask>("Email")
      .HasValue<PdfReportTask>("PdfReport")
      .HasValue<DataCleanupTask>("DataCleanup");
            modelBuilder.Entity<TaskExecutionLog>(entity =>
            {
                entity.ToTable("TaskExecutionLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ErrorMessage).HasMaxLength(1000);
                entity.HasOne(e => e.ScheduledTask)
                      .WithMany(t => t.ExecutionLogs)
                      .HasForeignKey(e => e.ScheduledTaskId);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
