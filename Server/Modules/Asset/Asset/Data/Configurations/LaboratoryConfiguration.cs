using Asset.Assets.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Asset.Data.Configurations;

public class LaboratoryConfiguration : IEntityTypeConfiguration<Laboratory>
{
    public void Configure(EntityTypeBuilder<Laboratory> builder)
    {
        builder.ToTable("Laboratories");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).UseIdentityColumn();

        builder.Property(l => l.LaboratoryName).HasMaxLength(200).IsRequired();
        builder.Property(l => l.RoomNo).HasMaxLength(50).IsRequired();
        builder.Property(l => l.TeacherId).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(1000).IsRequired();
    }
}
