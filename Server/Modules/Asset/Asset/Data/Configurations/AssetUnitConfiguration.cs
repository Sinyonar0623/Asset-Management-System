using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetUnitConfiguration : IEntityTypeConfiguration<AssetUnit>
{
    public void Configure(EntityTypeBuilder<AssetUnit> builder)
    {
        builder.ToTable("AssetUnits");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasColumnName("AssetUnitId");

        builder.Property(u => u.AssetTag).HasMaxLength(100).IsRequired();
        builder.Property(u => u.SerialNo).HasMaxLength(100).IsRequired();
        builder.Property(u => u.AvailabilityStatus).HasMaxLength(20).IsRequired();
        builder.Property(u => u.OperationalStatus).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Remark).HasMaxLength(500).IsRequired();
        builder.Property(u => u.ResponsibleUserId)
            .HasColumnName("OwnerId")
            .IsRequired(false);
        builder.HasIndex(u => u.AssetTag).IsUnique();
        builder.HasIndex(u => u.SerialNo).IsUnique();

        builder.Property<Guid?>("AssetId");
        builder.HasIndex("AssetId");

        builder.HasOne(a => a.Asset)
            .WithMany()
            .HasForeignKey("AssetId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Condition)
            .WithOne(c => c.AssetUnit)
            .HasForeignKey<AssetUnitCondition>("AssetUnitId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(a => a.Histories , histories =>
        {
            histories.ToTable("AssetUnitHistories");
            histories.WithOwner().HasForeignKey("AssetUnitId");
            histories.Property<int>("Id");
            histories.HasKey("AssetUnitId", "Id");
            histories.Property(h => h.ActionType).HasMaxLength(50).IsRequired();
            histories.Property(h => h.FromAvailabilityStatus).HasMaxLength(20).IsRequired(false);
            histories.Property(h => h.ToAvailabilityStatus).HasMaxLength(20).IsRequired(false);
            histories.Property(h => h.FromOperationalStatus).HasMaxLength(20).IsRequired(false);
            histories.Property(h => h.ToOperationalStatus).HasMaxLength(20).IsRequired(false);
            histories.Property(h => h.FromResponsibleUserId)
                .HasColumnName("FromOwnerId")
                .IsRequired(false);
            histories.Property(h => h.ToResponsibleUserId)
                .HasColumnName("ToOwnerId")
                .IsRequired(false);
            histories.Property(h => h.PerformedBy).IsRequired();
            histories.Property(h => h.PerformedAt).IsRequired();
            histories.Property(h => h.ApprovedBy).IsRequired(false);
            histories.Property(h => h.ApprovedAt).IsRequired(false);
            histories.Property(h => h.ReferenceNo).HasMaxLength(100).IsRequired(false);
            histories.Property(h => h.RequestId).IsRequired(false);
            histories.Property(h => h.Remark).HasMaxLength(500).IsRequired();
        });
    }
}
