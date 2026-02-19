using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetUnitConditionConfiguration : IEntityTypeConfiguration<AssetUnitCondition>
{
    public void Configure(EntityTypeBuilder<AssetUnitCondition> builder)
    {
        builder.ToTable("AssetUnitConditions");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.Property(c => c.Id).HasColumnName("AssetUnitConditionId");

        builder.Property(c => c.Reason).HasMaxLength(500).IsRequired();
        builder.Property(c => c.EffectiveFrom).IsRequired(false);
        builder.Property(c => c.IsEffectiveFromUnknown).IsRequired();
        builder.Property(c => c.EffectiveTo).IsRequired(false);
        builder.Property(c => c.IsEffectiveToUnknown).IsRequired();

        builder.Property<Guid>("AssetUnitId");
        builder.HasIndex("AssetUnitId").IsUnique();
    }
}
