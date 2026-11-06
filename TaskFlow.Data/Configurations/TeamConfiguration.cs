
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class TeamConfiguration
        : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            // Primary Key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.ProjectId)
                .IsRequired();

            builder.Property(t => t.LeaderId)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            // Project and Leader relationships are configured
            // in ProjectConfiguration and ApplicationUserConfiguration.
        }
    }
}
