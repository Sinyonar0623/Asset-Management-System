using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parameter.Parameters.Model;

namespace Parameter.Data.Configuration;

public class ParameterConfiguration : IEntityTypeConfiguration<Parameter.Parameters.Model.Parameter>
{
    public void Configure(EntityTypeBuilder<Parameter.Parameters.Model.Parameter> builder)
    {
        builder.ToTable("Parameters");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn();

        builder.Property(x => x.Group).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Value).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Active).IsRequired();

        builder.HasIndex(x => new { x.Group, x.Value }).IsUnique();
    }
}
