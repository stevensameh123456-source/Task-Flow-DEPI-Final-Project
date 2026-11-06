
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TaskApprovalConfiguration
        : IEntityTypeConfiguration<TaskApproval>
    {
        public void Configure(EntityTypeBuilder<TaskApproval> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            // Properties
            builder.Property(a => a.TaskId)
                .IsRequired();

            builder.Property(a => a.ReviewerId)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired();

            builder.Property(a => a.ReviewComment)
                .HasMaxLength(2000);

            // Task Relationship
            builder.HasOne(a => a.Task)
                .WithMany(t => t.Approvals)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Reviewer Relationship
            builder.HasOne(a => a.Reviewer)
                .WithMany()
                .HasForeignKey(a => a.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
