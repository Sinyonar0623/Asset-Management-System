using Auth.Authentication.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Data.Configurations;

public class AuthConfiguration : IEntityTypeConfiguration<UserName>
{
    public void Configure(EntityTypeBuilder<UserName> builder)
    {
        builder.ToTable("UserName");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasDefaultValueSql("NEWID()")
            .ValueGeneratedOnAdd();

        builder.Property<Guid>("RoleId").IsRequired();

        builder.HasOne(a => a.Role)
            .WithMany()
            .HasForeignKey("RoleId")
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Auth_RoleId");

    }
}
