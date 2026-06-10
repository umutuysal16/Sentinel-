using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentinel.Domain.Entities;

namespace Sentinel.Infrastructure.Persistence.Configurations;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(a => a.Name)
            .IsUnique();

        builder.Property(a => a.DeviceType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.Metadata)
            .HasColumnType("jsonb");

        builder.Ignore(a => a.DomainEvents);
    }
}
