using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset.Assets.Model.Asset>
{
    public void Configure(EntityTypeBuilder<Asset.Assets.Model.Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedOnAdd();
        builder.Property(m => m.Id).HasColumnName("AssetId");

        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(1000).IsRequired();
        builder.Property(m => m.Category).HasMaxLength(20).IsRequired();
        builder.Property(m => m.IsAvailable).IsRequired();

        builder.Property<Guid?>("LaboratoryId");
        builder.HasIndex("LaboratoryId");

        builder.HasOne(m => m.Laboratory)
            .WithMany()
            .HasForeignKey("LaboratoryId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
