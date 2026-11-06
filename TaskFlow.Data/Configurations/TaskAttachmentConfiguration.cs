
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TaskAttachmentConfiguration
        : IEntityTypeConfiguration<TaskAttachment>
    {
        public void Configure(EntityTypeBuilder<TaskAttachment> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            // Properties
            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.FileSize)
                .IsRequired();

            builder.Property(a => a.TaskId)
                .IsRequired();

            builder.Property(a => a.UploadedById)
                .IsRequired();

            builder.Property(a => a.UploadedAt)
                .IsRequired();

            // Task Relationship
            builder.HasOne(a => a.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // Uploaded By User Relationship
            builder.HasOne(a => a.UploadedBy)
                .WithMany()
                .HasForeignKey(a => a.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
