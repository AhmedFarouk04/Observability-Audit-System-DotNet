using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MetricSnapshotConfiguration : IEntityTypeConfiguration<MetricSnapshot>
{
    public void Configure(EntityTypeBuilder<MetricSnapshot> builder)
    {
        builder.ToTable("metric_snapshots");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Timestamp).IsRequired();
        builder.Property(x => x.TotalRequests).IsRequired();
        builder.Property(x => x.TotalErrors).IsRequired();
        builder.Property(x => x.AverageLatencyMs).IsRequired();
        builder.Property(x => x.P95LatencyMs).IsRequired();
        builder.Property(x => x.P99LatencyMs).IsRequired();

        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => x.Timestamp);
    }
}
