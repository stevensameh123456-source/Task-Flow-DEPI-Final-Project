using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TaskItemConfiguration
        : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.Description)
                .HasMaxLength(2000);

            builder.Property(t => t.StartDate)
                .IsRequired();

            builder.Property(t => t.DueDate)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.Priority)
                .IsRequired();

            builder.Property(t => t.TeamId)
                .IsRequired();

            builder.Property(t => t.BoardColumnId)
                .IsRequired();

            builder.Property(t => t.AssignedEmployeeId)
                .IsRequired();

            builder.Property(t => t.CreatedByLeaderId)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            // Team Relationship
            builder.HasOne(t => t.Team)
                .WithMany()
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Board Column Relationship
            builder.HasOne(t => t.BoardColumn)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.BoardColumnId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assigned Employee Relationship
            builder.HasOne(t => t.AssignedEmployee)
                .WithMany()
                .HasForeignKey(t => t.AssignedEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task Creator Relationship
            builder.HasOne(t => t.CreatedByLeader)
                .WithMany()
                .HasForeignKey(t => t.CreatedByLeaderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}