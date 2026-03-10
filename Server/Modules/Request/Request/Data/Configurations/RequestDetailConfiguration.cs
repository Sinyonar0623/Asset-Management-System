namespace request.Data.Configurations;

public class RequestDetailConfiguration : IEntityTypeConfiguration<RequestDetail>
{
    public void Configure(EntityTypeBuilder<RequestDetail> builder)
    {
        builder.ToTable("RequestDetails");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property<Guid>("RequestId").IsRequired(); // shadow FK
        builder.HasIndex("RequestId").IsUnique();

        builder.Property(x => x.Purpose).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.IssueDescription).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.RetireReason).HasMaxLength(2000).IsRequired(false);
        builder.Property(x => x.ExtraNote).HasMaxLength(2000).IsRequired(false);
    }
}
