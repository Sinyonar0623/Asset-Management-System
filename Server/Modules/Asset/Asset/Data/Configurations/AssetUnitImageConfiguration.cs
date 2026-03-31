using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetUnitImageConfiguration : IEntityTypeConfiguration<AssetUnitImage>
{
    public void Configure(EntityTypeBuilder<AssetUnitImage> builder)
    {
        builder.ToTable("AssetUnitImages");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();
        builder.Property(i => i.Id).HasColumnName("AssetUnitImageId");

        builder.Property(i => i.ImageUrl).HasMaxLength(1000).IsRequired();
        builder.Property(i => i.FileName).HasMaxLength(255).IsRequired(false);
        builder.Property(i => i.ContentType).HasMaxLength(100).IsRequired(false);
        builder.Property(i => i.FileSizeBytes).IsRequired(false);
        builder.Property(i => i.IsPrimary).IsRequired();

        builder.Property<Guid>("AssetUnitId");
        builder.HasIndex("AssetUnitId");

        builder.HasOne(i => i.AssetUnit)
            .WithMany(u => u.Images)
            .HasForeignKey("AssetUnitId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
