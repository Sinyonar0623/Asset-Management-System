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
        builder.Property(u => u.OwnerId).IsRequired();
        builder.HasIndex(u => u.AssetTag).IsUnique();
        builder.HasIndex(u => u.SerialNo).IsUnique();

        builder.Property<Guid>("AssetId");
        builder.HasIndex("AssetId");

        builder.HasOne(a => a.Asset)
            .WithMany()
            .HasForeignKey("AssetId")
            .OnDelete(DeleteBehavior.Restrict);

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
        });
    }
}
