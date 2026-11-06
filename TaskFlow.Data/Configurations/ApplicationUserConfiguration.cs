
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class ApplicationUserConfiguration
        : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(
            EntityTypeBuilder<ApplicationUser> builder)
        {
            // Properties
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            // Unique Email Index
            builder.HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("EmailIndex")
                .HasFilter("[NormalizedEmail] IS NOT NULL");

            // Employee Team Relationship
            builder.HasOne(u => u.Team)
                .WithMany(t => t.Employees)
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Team Leader Relationship
            builder.HasMany(u => u.LedTeams)
                .WithOne(t => t.Leader)
                .HasForeignKey(t => t.LeaderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Project Manager Relationship
            builder.HasMany(u => u.ManagedProjects)
                .WithOne(p => p.ProjectManager)
                .HasForeignKey(p => p.ProjectManagerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
