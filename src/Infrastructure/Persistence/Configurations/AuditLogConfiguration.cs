using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.Timestamp).IsRequired();

        builder.Property(x => x.Action).HasMaxLength(200).IsRequired();
        builder.Property(x => x.UserId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UserEmail).HasMaxLength(200).IsRequired();
        builder.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.EntityId).HasMaxLength(100);
        builder.Property(x => x.IpAddress).HasMaxLength(50).IsRequired();
        builder.Property(x => x.CorrelationId).HasMaxLength(100).IsRequired();
        builder.Property(x => x.UserAgent).HasMaxLength(500);
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.Property(x => x.DurationMs).IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.OldValues);
        builder.Property(x => x.NewValues);

        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => x.CorrelationId);
        builder.HasIndex(x => x.Action);
        builder.HasIndex(x => new { x.EntityType, x.EntityId });
    }
}
