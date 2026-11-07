using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Data.Entities;

namespace TaskFlow.Data.Configurations
{
    public class BoardColumnConfiguration
        : IEntityTypeConfiguration<BoardColumn>
    {
        public void Configure(EntityTypeBuilder<BoardColumn> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Position)
                .IsRequired();

            builder.Property(c => c.WipLimit)
                .IsRequired(false);

            builder.HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new { c.BoardId, c.Position })
                .IsUnique();
        }
    }
}