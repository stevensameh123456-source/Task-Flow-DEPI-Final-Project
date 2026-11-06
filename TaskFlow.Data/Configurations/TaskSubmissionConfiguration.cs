
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TaskSubmissionConfiguration
        : IEntityTypeConfiguration<TaskSubmission>
    {
        public void Configure(EntityTypeBuilder<TaskSubmission> builder)
        {
            // Primary Key
            builder.HasKey(s => s.Id);

            // Properties
            builder.Property(s => s.TaskId)
                .IsRequired();

            builder.Property(s => s.EmployeeId)
                .IsRequired();

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(s => s.SubmittedAt)
                .IsRequired();

            builder.Property(s => s.Status)
                .IsRequired();

            // Task Relationship
            builder.HasOne(s => s.Task)
                .WithMany(t => t.Submissions)
                .HasForeignKey(s => s.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Relationship
            builder.HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
