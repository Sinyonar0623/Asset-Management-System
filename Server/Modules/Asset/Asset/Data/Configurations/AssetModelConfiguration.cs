using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetModelConfiguration : IEntityTypeConfiguration<AssetModel>
{
    public void Configure(EntityTypeBuilder<AssetModel> builder)
    {
        builder.ToTable("AssetModels");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).UseIdentityColumn();

        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(1000).IsRequired();
        builder.Property(m => m.Category).HasMaxLength(20).IsRequired();
        builder.Property(m => m.IsAvailable).IsRequired();

        builder.HasOne(m => m.Laboratory)
            .WithMany()
            .HasForeignKey(m => m.LaboratoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_AssetModels_Laboratories_LaboratoryId");

        builder.HasIndex(m => m.LaboratoryId);
        builder.HasIndex(m => new { m.LaboratoryId, m.Name }).IsUnique();
    }
}