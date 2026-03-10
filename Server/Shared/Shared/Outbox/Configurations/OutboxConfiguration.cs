using Microsoft.EntityFrameworkCore;

namespace Shared.Data.Outbox.Configuration;

public sealed class OutboxConfiguration(bool excludeFromMigrations = false) : IEntityTypeConfiguration<Shared.Outbox.Model.Outbox>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Shared.Outbox.Model.Outbox> builder)
    {
        builder.ToTable("OutboxMessages", t =>
        {
            if (excludeFromMigrations) t.ExcludeFromMigrations();
        });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Type).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Payload).IsRequired();

        builder.Property(x => x.OccurredOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .ValueGeneratedOnAdd();
    }
}
