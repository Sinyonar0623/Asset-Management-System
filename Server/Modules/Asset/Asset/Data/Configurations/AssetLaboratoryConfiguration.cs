using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetLaboratoryConfiguration : IEntityTypeConfiguration<AssetLaboratory>
{
    public void Configure(EntityTypeBuilder<AssetLaboratory> builder)
    {
        builder.ToTable("Laboratories");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).UseIdentityColumn();
    }
}