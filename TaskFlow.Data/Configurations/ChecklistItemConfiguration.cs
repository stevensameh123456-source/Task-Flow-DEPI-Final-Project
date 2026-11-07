using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class ChecklistItemConfiguration
        : IEntityTypeConfiguration<ChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistItem> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.IsCompleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.Position)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasOne(c => c.Task)
                .WithMany(t => t.ChecklistItems)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.TaskId, c.Position })
                .IsUnique();
        }
    }
}