using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TaskTagConfiguration
        : IEntityTypeConfiguration<TaskTag>
    {
        public void Configure(EntityTypeBuilder<TaskTag> builder)
        {
            builder.HasKey(tt => new { tt.TaskId, tt.TagId });

            builder.HasOne(tt => tt.Task)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}