using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class WorkspaceMemberConfiguration
        : IEntityTypeConfiguration<WorkspaceMember>
    {
        public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
        {
            // Primary Key
            builder.HasKey(m => m.Id);

            // Properties
            builder.Property(m => m.WorkspaceId)
                .IsRequired();

            builder.Property(m => m.UserId)
                .IsRequired();

            builder.Property(m => m.Role)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(m => m.JoinedAt)
                .IsRequired();

            // Prevent duplicate membership
            builder.HasIndex(m => new { m.WorkspaceId, m.UserId })
                .IsUnique();

            // Workspace Relationship
            builder.HasOne(m => m.Workspace)
                .WithMany(w => w.Members)
                .HasForeignKey(m => m.WorkspaceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // User Relationship
            builder.HasOne(m => m.User)
                .WithMany(u => u.WorkspaceMembers)
                .HasForeignKey(m => m.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}