using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Color)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(t => t.ProjectId)
                .IsRequired();

            builder.HasOne(t => t.Project)
                .WithMany(p => p.Tags)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => new { t.ProjectId, t.Name })
                .IsUnique();
        }
    }
}