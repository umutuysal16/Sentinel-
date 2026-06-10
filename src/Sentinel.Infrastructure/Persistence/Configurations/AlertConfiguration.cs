using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("alerts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.RiskScore)
            .IsRequired();

        builder.Property(a => a.Severity)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Category)
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(a => a.AiExplanation)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(a => a.AcknowledgedBy)
            .HasMaxLength(200);

        builder.HasIndex(a => new { a.Severity, a.IsAcknowledged });
        builder.HasIndex(a => a.CreatedAt);

        builder.HasOne(a => a.LogEntry)
            .WithMany()
            .HasForeignKey(a => a.LogEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable(t => t.HasCheckConstraint("CK_Alert_RiskScore", "\"RiskScore\" >= 1 AND \"RiskScore\" <= 10"));

        builder.Ignore(a => a.DomainEvents);
    }
}
