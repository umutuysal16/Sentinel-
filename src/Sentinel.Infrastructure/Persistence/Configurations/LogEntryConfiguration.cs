using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configurations;

public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
{
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.ToTable("log_entries");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Source)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(l => l.Level)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(l => l.Message)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(l => l.StackTrace)
            .HasMaxLength(8000);

        builder.Property(l => l.Properties)
            .HasColumnType("jsonb");

        builder.HasIndex(l => new { l.AgentId, l.Timestamp });
        builder.HasIndex(l => l.Level);
        builder.HasIndex(l => l.Timestamp);

        builder.HasOne(l => l.Agent)
            .WithMany(a => a.LogEntries)
            .HasForeignKey(l => l.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(l => l.DomainEvents);
    }
}
