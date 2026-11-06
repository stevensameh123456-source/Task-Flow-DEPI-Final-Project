
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Context
{
    public class AppDbContext : IdentityDbContext<
        ApplicationUser,
        IdentityRole<int>,
        int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =====================================
        // DbSets
        // =====================================

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        public DbSet<TaskAttachment> TaskAttachments => Set<TaskAttachment>();
        public DbSet<TaskSubmission> TaskSubmissions => Set<TaskSubmission>();
        public DbSet<TaskApproval> TaskApprovals => Set<TaskApproval>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<EmployeePerformance> EmployeePerformances => Set<EmployeePerformance>();
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

        // =====================================
        // Model Configurations
        // =====================================

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply ASP.NET Core Identity configurations
            base.OnModelCreating(builder);

            // Automatically discover and apply
            // all IEntityTypeConfiguration classes
            // from the current assembly.

            builder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly
            );
        }
    }
}
