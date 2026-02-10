using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class AssetsConfiguration : IEntityTypeConfiguration<Assets.Model.Asset>
{
    public void Configure(EntityTypeBuilder<Assets.Model.Asset> builder)
    {
        builder.ToTable("Assets");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).UseIdentityColumn();

        builder.Property<long>("LaboratoryId").IsRequired();

        builder.HasOne(a => a.Laboratory)
               .WithMany()
               .HasForeignKey("LaboratoryId")
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("FK_Assets_Laboratory_LaboratoryId");

        builder.HasIndex("LaboratoryId");

        builder.OwnsMany(a => a.Histories, histories =>
        {
            histories.ToTable("AssetHistories");

            histories.WithOwner().HasForeignKey("AssetId");

            histories.Property<long>("Id").UseIdentityColumn();
            histories.HasKey("Id");

            histories.Property(x => x.Purpose).HasMaxLength(200).IsRequired();
            histories.Property(x => x.Remark).HasMaxLength(500).IsRequired(false);
            histories.Property(x => x.ApproveBy).IsRequired();
            histories.Property(x => x.ApproveAt).IsRequired();

            histories.HasIndex("AssetId");
        });

        builder.Navigation(a => a.Histories)
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
