
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class EmployeePerformanceConfiguration
        : IEntityTypeConfiguration<EmployeePerformance>
    {
        public void Configure(
            EntityTypeBuilder<EmployeePerformance> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.EmployeeId)
                .IsRequired();

            builder.Property(e => e.CompletedTasks)
                .IsRequired();

            builder.Property(e => e.PendingTasks)
                .IsRequired();

            builder.Property(e => e.RejectedTasks)
                .IsRequired();

            builder.Property(e => e.CompletionRate)
                .IsRequired();

            builder.Property(e => e.CalculatedAt)
                .IsRequired();

            // Employee Relationship
            builder.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
