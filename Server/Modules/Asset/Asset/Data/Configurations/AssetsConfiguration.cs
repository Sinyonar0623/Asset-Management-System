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

        builder.Property(a => a.RealWorldId).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Brand).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(200).IsRequired();
        builder.Property(a => a.SerialNo).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1000).IsRequired();
        builder.Property(a => a.Type).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Status).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Remark).HasMaxLength(500).IsRequired();
        builder.Property(a => a.OwnerId).IsRequired();
        builder.Property(a => a.Amount).IsRequired();

        builder.HasIndex(a => a.RealWorldId).IsUnique();
        builder.HasIndex(a => a.SerialNo).IsUnique();

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

        builder.OwnsMany(a => a.Components, components =>
        {
            components.ToTable("AssetComponents");

            components.WithOwner().HasForeignKey("AssetId");

            components.Property<long>("Id").UseIdentityColumn();
            components.HasKey("Id");

            components.Property(x => x.RealWorldId).HasMaxLength(100).IsRequired();
            components.Property(x => x.Brand).HasMaxLength(100).IsRequired();
            components.Property(x => x.Name).HasMaxLength(200).IsRequired();
            components.Property(x => x.SerialNo).HasMaxLength(100).IsRequired();
            components.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            components.Property(x => x.Type).HasMaxLength(100).IsRequired();
            components.Property(x => x.Remark).HasMaxLength(500).IsRequired();

            components.HasIndex("AssetId");
            components.HasIndex("AssetId", nameof(Assets.Model.AssetComponent.SerialNo)).IsUnique();
        });

        builder.Navigation(a => a.Histories)
               .HasField("_histories")
               .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(a => a.Components)
               .HasField("_component")
               .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
