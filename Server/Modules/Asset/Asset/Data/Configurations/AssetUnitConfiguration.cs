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
        builder.Property(u => u.Id).UseIdentityColumn();

        builder.Property(u => u.AssetTag).HasMaxLength(100).IsRequired();
        builder.Property(u => u.SerialNo).HasMaxLength(100).IsRequired();
        builder.Property(u => u.AvailabilityStatus).HasMaxLength(20).IsRequired();
        builder.Property(u => u.OperationalStatus).HasMaxLength(20).IsRequired();
        builder.Property(u => u.Remark).HasMaxLength(500).IsRequired();
        builder.Property(u => u.OwnerId).IsRequired();

        builder.HasOne(u => u.AssetModel)
            .WithMany()
            .HasForeignKey(u => u.AssetModelId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_AssetUnits_AssetModels_AssetModelId");

        builder.HasIndex(u => u.AssetModelId);
        builder.HasIndex(u => u.AssetTag).IsUnique();
        builder.HasIndex(u => u.SerialNo).IsUnique();

        builder.OwnsMany(u => u.Histories, histories =>
        {
            histories.ToTable("AssetHistories");

            histories.WithOwner().HasForeignKey("AssetUnitId");

            histories.Property<long>("Id").UseIdentityColumn();
            histories.HasKey("Id");

            histories.Property(x => x.Purpose).HasMaxLength(200).IsRequired();
            histories.Property(x => x.Remark).HasMaxLength(500).IsRequired();
            histories.Property(x => x.ApproveBy).IsRequired();
            histories.Property(x => x.ApproveAt).IsRequired();

            histories.HasIndex("AssetUnitId");
        });

        builder.Navigation(u => u.Histories)
            .HasField("_histories")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
