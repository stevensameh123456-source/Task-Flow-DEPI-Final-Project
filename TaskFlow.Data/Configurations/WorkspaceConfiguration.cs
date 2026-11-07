using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class WorkspaceConfiguration
        : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            // Primary Key
            builder.HasKey(w => w.Id);

            // Properties
            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(w => w.Description)
                .HasMaxLength(1000);

            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.OwnerId)
                .IsRequired();

            // Workspace Owner Relationship
            builder.HasOne(w => w.Owner)
                .WithMany()
                .HasForeignKey(w => w.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Workspace Projects Relationship
            builder.HasMany(w => w.Projects)
                .WithOne(p => p.Workspace)
                .HasForeignKey(p => p.WorkspaceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Workspace Members Relationship
            builder.HasMany(w => w.Members)
                .WithOne(m => m.Workspace)
                .HasForeignKey(m => m.WorkspaceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}